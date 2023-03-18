using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Storage.EntitySystems;
using Content.Server.Stunnable;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.PneumaticCannon;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Tools.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.PneumaticCannon
{
	// Token: 0x020002D0 RID: 720
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PneumaticCannonSystem : SharedPneumaticCannonSystem
	{
		// Token: 0x06000E79 RID: 3705 RVA: 0x00049BE0 File Offset: 0x00047DE0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PneumaticCannonComponent, InteractUsingEvent>(new ComponentEventHandler<PneumaticCannonComponent, InteractUsingEvent>(this.OnInteractUsing), new Type[]
			{
				typeof(StorageSystem)
			}, null);
			base.SubscribeLocalEvent<PneumaticCannonComponent, GunShotEvent>(new ComponentEventRefHandler<PneumaticCannonComponent, GunShotEvent>(this.OnShoot), null, null);
			base.SubscribeLocalEvent<PneumaticCannonComponent, ContainerIsInsertingAttemptEvent>(new ComponentEventHandler<PneumaticCannonComponent, ContainerIsInsertingAttemptEvent>(this.OnContainerInserting), null, null);
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x00049C44 File Offset: 0x00047E44
		private void OnInteractUsing(EntityUid uid, PneumaticCannonComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool))
			{
				return;
			}
			if (!tool.Qualities.Contains(component.ToolModifyPower))
			{
				return;
			}
			int val = (int)component.Power;
			val = (val + 1) % 3;
			component.Power = (PneumaticCannonPower)val;
			this.Popup.PopupEntity(Loc.GetString("pneumatic-cannon-component-change-power", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("power", component.Power.ToString())
			}), uid, args.User, PopupType.Small);
			GunComponent gun;
			if (base.TryComp<GunComponent>(uid, ref gun))
			{
				gun.ProjectileSpeed = this.GetProjectileSpeedFromPower(component);
			}
			args.Handled = true;
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x00049CF8 File Offset: 0x00047EF8
		private void OnContainerInserting(EntityUid uid, PneumaticCannonComponent component, ContainerIsInsertingAttemptEvent args)
		{
			if (args.Container.ID != "gas_tank")
			{
				return;
			}
			GasTankComponent gas;
			if (!base.TryComp<GasTankComponent>(args.EntityUid, ref gas))
			{
				return;
			}
			if (gas.Air.TotalMoles >= component.GasUsage)
			{
				return;
			}
			args.Cancel();
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x00049D48 File Offset: 0x00047F48
		private void OnShoot(EntityUid uid, PneumaticCannonComponent component, ref GunShotEvent args)
		{
			GasTankComponent gas = this.GetGas(uid);
			if (gas == null)
			{
				return;
			}
			StatusEffectsComponent status;
			if (base.TryComp<StatusEffectsComponent>(args.User, ref status) && component.Power == PneumaticCannonPower.High)
			{
				this._stun.TryParalyze(args.User, TimeSpan.FromSeconds((double)component.HighPowerStunTime), true, status);
				this.Popup.PopupEntity(Loc.GetString("pneumatic-cannon-component-power-stun", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("cannon", component.Owner)
				}), uid, args.User, PopupType.Small);
			}
			GasMixture environment = this._atmos.GetContainingMixture(component.Owner, false, true, null);
			GasMixture removed = this._gasTank.RemoveAir(gas, component.GasUsage);
			if (environment != null && removed != null)
			{
				this._atmos.Merge(environment, removed);
			}
			if (gas.Air.TotalMoles >= component.GasUsage)
			{
				return;
			}
			EntityUid? entityUid;
			this._slots.TryEject(uid, "gas_tank", new EntityUid?(args.User), out entityUid, null, false);
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x00049E4C File Offset: 0x0004804C
		[NullableContext(2)]
		private GasTankComponent GetGas(EntityUid uid)
		{
			IContainer container;
			if (this.Container.TryGetContainer(uid, "gas_tank", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null)
				{
					EntityUid? containedEntity = slot.ContainedEntity;
					if (containedEntity != null)
					{
						EntityUid contained = containedEntity.GetValueOrDefault();
						GasTankComponent gasTank;
						if (!base.TryComp<GasTankComponent>(contained, ref gasTank))
						{
							return null;
						}
						return gasTank;
					}
				}
			}
			return null;
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x00049EA4 File Offset: 0x000480A4
		private float GetProjectileSpeedFromPower(PneumaticCannonComponent component)
		{
			PneumaticCannonPower power = component.Power;
			float result;
			if (power != PneumaticCannonPower.Medium)
			{
				if (power == PneumaticCannonPower.High)
				{
					result = component.BaseProjectileSpeed * 4f;
				}
				else
				{
					result = component.BaseProjectileSpeed * 0.5f;
				}
			}
			else
			{
				result = component.BaseProjectileSpeed;
			}
			return result;
		}

		// Token: 0x04000891 RID: 2193
		[Dependency]
		private readonly AtmosphereSystem _atmos;

		// Token: 0x04000892 RID: 2194
		[Dependency]
		private readonly GasTankSystem _gasTank;

		// Token: 0x04000893 RID: 2195
		[Dependency]
		private readonly StunSystem _stun;

		// Token: 0x04000894 RID: 2196
		[Dependency]
		private readonly ItemSlotsSystem _slots;
	}
}
