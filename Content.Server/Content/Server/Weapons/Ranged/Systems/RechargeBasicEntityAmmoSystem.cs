using System;
using System.Runtime.CompilerServices;
using Content.Server.Weapons.Ranged.Components;
using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Weapons.Ranged.Systems
{
	// Token: 0x020000B1 RID: 177
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RechargeBasicEntityAmmoSystem : EntitySystem
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x0000F6A8 File Offset: 0x0000D8A8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ComponentInit>(new ComponentEventHandler<RechargeBasicEntityAmmoComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ExaminedEvent>(new ComponentEventHandler<RechargeBasicEntityAmmoComponent, ExaminedEvent>(this.OnExamined), null, null);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent> valueTuple in base.EntityQuery<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent>(false))
			{
				RechargeBasicEntityAmmoComponent recharge = valueTuple.Item1;
				BasicEntityAmmoProviderComponent ammo = valueTuple.Item2;
				int? count = ammo.Count;
				if (count != null)
				{
					count = ammo.Count;
					int? capacity = ammo.Capacity;
					if (!(count.GetValueOrDefault() == capacity.GetValueOrDefault() & count != null == (capacity != null)))
					{
						recharge.AccumulatedFrameTime += frameTime;
						if (recharge.AccumulatedFrameTime >= recharge.NextRechargeTime)
						{
							recharge.AccumulatedFrameTime -= recharge.NextRechargeTime;
							this.UpdateCooldown(recharge);
							if (this._gun.UpdateBasicEntityAmmoCount(ammo.Owner, ammo.Count.Value + 1, ammo))
							{
								SoundSystem.Play(recharge.RechargeSound.GetSound(null, null), Filter.Pvs(recharge.Owner, 2f, null, null, null), recharge.Owner, new AudioParams?(recharge.RechargeSound.Params));
							}
						}
					}
				}
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000F820 File Offset: 0x0000DA20
		private void OnInit(EntityUid uid, RechargeBasicEntityAmmoComponent component, ComponentInit args)
		{
			this.UpdateCooldown(component);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000F82C File Offset: 0x0000DA2C
		private void OnExamined(EntityUid uid, RechargeBasicEntityAmmoComponent component, ExaminedEvent args)
		{
			BasicEntityAmmoProviderComponent ammo;
			if (base.TryComp<BasicEntityAmmoProviderComponent>(uid, ref ammo))
			{
				int? count = ammo.Count;
				int? capacity = ammo.Capacity;
				if (!(count.GetValueOrDefault() == capacity.GetValueOrDefault() & count != null == (capacity != null)))
				{
					float timeLeft = component.NextRechargeTime - component.AccumulatedFrameTime;
					args.PushMarkup(Loc.GetString("recharge-basic-entity-ammo-can-recharge", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("seconds", Math.Round((double)timeLeft, 1))
					}));
					return;
				}
			}
			args.PushMarkup(Loc.GetString("recharge-basic-entity-ammo-full"));
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000F8C8 File Offset: 0x0000DAC8
		private void UpdateCooldown(RechargeBasicEntityAmmoComponent component)
		{
			component.NextRechargeTime = this._random.NextFloat(component.MinRechargeCooldown, component.MaxRechargeCooldown);
		}

		// Token: 0x040001EB RID: 491
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040001EC RID: 492
		[Dependency]
		private readonly SharedGunSystem _gun;
	}
}
