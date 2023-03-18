using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Gravity;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x02000048 RID: 72
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGunSystem : EntitySystem
	{
		// Token: 0x0600009D RID: 157 RVA: 0x0000352C File Offset: 0x0000172C
		protected virtual void InitializeBallistic()
		{
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<BallisticAmmoProviderComponent, ComponentInit>(this.OnBallisticInit), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, MapInitEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, MapInitEvent>(this.OnBallisticMapInit), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, TakeAmmoEvent>(this.OnBallisticTakeAmmo), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, GetAmmoCountEvent>(this.OnBallisticAmmoCount), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, ComponentGetState>(this.OnBallisticGetState), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, ComponentHandleState>(this.OnBallisticHandleState), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, ExaminedEvent>(this.OnBallisticExamine), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>(this.OnBallisticVerb), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, InteractUsingEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, InteractUsingEvent>(this.OnBallisticInteractUsing), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, AfterInteractEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, AfterInteractEvent>(this.OnBallisticAfterInteract), null, null);
			base.SubscribeLocalEvent<BallisticAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, UseInHandEvent>(this.OnBallisticUse), null, null);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00003615 File Offset: 0x00001815
		private void OnBallisticUse(EntityUid uid, BallisticAmmoProviderComponent component, UseInHandEvent args)
		{
			this.ManualCycle(component, base.Transform(uid).MapPosition, new EntityUid?(args.User));
			args.Handled = true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000363C File Offset: 0x0000183C
		private void OnBallisticInteractUsing(EntityUid uid, BallisticAmmoProviderComponent component, InteractUsingEvent args)
		{
			if (!args.Handled)
			{
				EntityWhitelist whitelist = component.Whitelist;
				if (whitelist != null && whitelist.IsValid(args.Used, this.EntityManager))
				{
					if (this.GetBallisticShots(component) >= component.Capacity)
					{
						return;
					}
					component.Entities.Add(args.Used);
					component.Container.Insert(args.Used, null, null, null, null, null);
					this.Audio.PlayPredicted(component.SoundInsert, uid, new EntityUid?(args.User), null);
					args.Handled = true;
					this.UpdateBallisticAppearance(component);
					base.Dirty(component, null);
					return;
				}
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000036EC File Offset: 0x000018EC
		private void OnBallisticAfterInteract(EntityUid uid, BallisticAmmoProviderComponent component, AfterInteractEvent args)
		{
			SharedGunSystem.<>c__DisplayClass3_0 CS$<>8__locals1;
			CS$<>8__locals1.args = args;
			CS$<>8__locals1.<>4__this = this;
			BallisticAmmoProviderComponent targetComponent;
			if (CS$<>8__locals1.args.Handled || !component.MayTransfer || !this.Timing.IsFirstTimePredicted || CS$<>8__locals1.args.Target == null || CS$<>8__locals1.args.Used == CS$<>8__locals1.args.Target || base.Deleted(CS$<>8__locals1.args.Target) || !base.TryComp<BallisticAmmoProviderComponent>(CS$<>8__locals1.args.Target, ref targetComponent) || targetComponent.Whitelist == null)
			{
				return;
			}
			CS$<>8__locals1.args.Handled = true;
			if (targetComponent.Entities.Count + targetComponent.UnspawnedCount == targetComponent.Capacity)
			{
				this.Popup(Loc.GetString("gun-ballistic-transfer-target-full", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", CS$<>8__locals1.args.Target)
				}), CS$<>8__locals1.args.Target, new EntityUid?(CS$<>8__locals1.args.User));
				return;
			}
			if (component.Entities.Count + component.UnspawnedCount == 0)
			{
				this.Popup(Loc.GetString("gun-ballistic-transfer-empty", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", CS$<>8__locals1.args.Used)
				}), new EntityUid?(CS$<>8__locals1.args.Used), new EntityUid?(CS$<>8__locals1.args.User));
				return;
			}
			List<IShootable> ammo = new List<IShootable>();
			TakeAmmoEvent evTakeAmmo = new TakeAmmoEvent(1, ammo, base.Transform(CS$<>8__locals1.args.Used).Coordinates, new EntityUid?(CS$<>8__locals1.args.User));
			base.RaiseLocalEvent<TakeAmmoEvent>(CS$<>8__locals1.args.Used, evTakeAmmo, false);
			foreach (IShootable shootable in ammo)
			{
				AmmoComponent cast = shootable as AmmoComponent;
				if (cast != null)
				{
					if (!targetComponent.Whitelist.IsValid(cast.Owner, null))
					{
						this.Popup(Loc.GetString("gun-ballistic-transfer-invalid", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("ammoEntity", cast.Owner),
							new ValueTuple<string, object>("targetEntity", CS$<>8__locals1.args.Target.Value)
						}), new EntityUid?(CS$<>8__locals1.args.Used), new EntityUid?(CS$<>8__locals1.args.User));
						this.<OnBallisticAfterInteract>g__SimulateInsertAmmo|3_0(cast.Owner, CS$<>8__locals1.args.Used, base.Transform(CS$<>8__locals1.args.Used).Coordinates, ref CS$<>8__locals1);
					}
					else
					{
						this.<OnBallisticAfterInteract>g__SimulateInsertAmmo|3_0(cast.Owner, CS$<>8__locals1.args.Target.Value, base.Transform(CS$<>8__locals1.args.Target.Value).Coordinates, ref CS$<>8__locals1);
					}
					if (cast.Owner.IsClientSide())
					{
						base.Del(cast.Owner);
					}
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00003A5C File Offset: 0x00001C5C
		private void OnBallisticVerb(EntityUid uid, BallisticAmmoProviderComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			args.Verbs.Add(new Verb
			{
				Text = Loc.GetString("gun-ballistic-cycle"),
				Disabled = (this.GetBallisticShots(component) == 0),
				Act = delegate()
				{
					this.ManualCycle(component, this.Transform(uid).MapPosition, new EntityUid?(args.User));
				}
			});
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00003B00 File Offset: 0x00001D00
		private void OnBallisticExamine(EntityUid uid, BallisticAmmoProviderComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("gun-magazine-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("count", this.GetBallisticShots(component))
			}));
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003B60 File Offset: 0x00001D60
		private void ManualCycle(BallisticAmmoProviderComponent component, MapCoordinates coordinates, EntityUid? user = null)
		{
			GunComponent gunComp;
			if (base.TryComp<GunComponent>(component.Owner, ref gunComp) && gunComp != null && gunComp.FireRate > 0f)
			{
				gunComp.NextFire = this.Timing.CurTime + TimeSpan.FromSeconds((double)(1f / gunComp.FireRate));
			}
			base.Dirty(component, null);
			this.Audio.PlayPredicted(component.SoundRack, component.Owner, user, null);
			bool ballisticShots = this.GetBallisticShots(component) != 0;
			component.Cycled = true;
			this.Cycle(component, coordinates);
			string text = Loc.GetString((!ballisticShots) ? "gun-ballistic-cycled-empty" : "gun-ballistic-cycled");
			this.Popup(text, new EntityUid?(component.Owner), user);
			this.UpdateBallisticAppearance(component);
			this.UpdateAmmoCount(component.Owner);
		}

		// Token: 0x060000A4 RID: 164
		protected abstract void Cycle(BallisticAmmoProviderComponent component, MapCoordinates coordinates);

		// Token: 0x060000A5 RID: 165 RVA: 0x00003C2F File Offset: 0x00001E2F
		private void OnBallisticGetState(EntityUid uid, BallisticAmmoProviderComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.BallisticAmmoProviderComponentState
			{
				UnspawnedCount = component.UnspawnedCount,
				Entities = component.Entities,
				Cycled = component.Cycled
			};
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00003C60 File Offset: 0x00001E60
		private void OnBallisticHandleState(EntityUid uid, BallisticAmmoProviderComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.BallisticAmmoProviderComponentState state = args.Current as SharedGunSystem.BallisticAmmoProviderComponentState;
			if (state == null)
			{
				return;
			}
			component.Cycled = state.Cycled;
			component.UnspawnedCount = state.UnspawnedCount;
			component.Entities.Clear();
			foreach (EntityUid ent in state.Entities)
			{
				component.Entities.Add(ent);
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00003CEC File Offset: 0x00001EEC
		private void OnBallisticInit(EntityUid uid, BallisticAmmoProviderComponent component, ComponentInit args)
		{
			component.Container = this.Containers.EnsureContainer<Container>(uid, "ballistic-ammo", null);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00003D06 File Offset: 0x00001F06
		private void OnBallisticMapInit(EntityUid uid, BallisticAmmoProviderComponent component, MapInitEvent args)
		{
			if (component.FillProto != null)
			{
				component.UnspawnedCount = Math.Max(0, component.Capacity - component.Container.ContainedEntities.Count);
				base.Dirty(component, null);
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00003D3B File Offset: 0x00001F3B
		protected int GetBallisticShots(BallisticAmmoProviderComponent component)
		{
			return component.Entities.Count + component.UnspawnedCount;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003D50 File Offset: 0x00001F50
		private void OnBallisticTakeAmmo(EntityUid uid, BallisticAmmoProviderComponent component, TakeAmmoEvent args)
		{
			int i = 0;
			while (i < args.Shots && component.Cycled)
			{
				if (component.Entities.Count > 0)
				{
					List<EntityUid> entities = component.Entities;
					EntityUid entity = entities[entities.Count - 1];
					args.Ammo.Add(base.EnsureComp<AmmoComponent>(entity));
					if (!component.AutoCycle)
					{
						return;
					}
					component.Entities.RemoveAt(component.Entities.Count - 1);
					component.Container.Remove(entity, null, null, null, true, false, null, null);
				}
				else if (component.UnspawnedCount > 0)
				{
					component.UnspawnedCount--;
					EntityUid entity = base.Spawn(component.FillProto, args.Coordinates);
					args.Ammo.Add(base.EnsureComp<AmmoComponent>(entity));
					if (base.HasComp<CartridgeAmmoComponent>(entity) && !component.AutoCycle)
					{
						if (!entity.IsClientSide())
						{
							component.Entities.Add(entity);
							component.Container.Insert(entity, null, null, null, null, null);
						}
						else
						{
							component.UnspawnedCount++;
						}
					}
				}
				if (!component.AutoCycle)
				{
					component.Cycled = false;
				}
				i++;
			}
			this.UpdateBallisticAppearance(component);
			base.Dirty(component, null);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00003E9F File Offset: 0x0000209F
		private void OnBallisticAmmoCount(EntityUid uid, BallisticAmmoProviderComponent component, ref GetAmmoCountEvent args)
		{
			args.Count = this.GetBallisticShots(component);
			args.Capacity = component.Capacity;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00003EBC File Offset: 0x000020BC
		private void UpdateBallisticAppearance(BallisticAmmoProviderComponent component)
		{
			AppearanceComponent appearance;
			if (!this.Timing.IsFirstTimePredicted || !base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoCount, this.GetBallisticShots(component), appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoMax, component.Capacity, appearance);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003F30 File Offset: 0x00002130
		protected virtual void InitializeBasicEntity()
		{
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, ComponentInit>(this.OnBasicEntityInit), null, null);
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, TakeAmmoEvent>(this.OnBasicEntityTakeAmmo), null, null);
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>(this.OnBasicEntityAmmoCount), null, null);
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, ComponentGetState>(this.OnBasicEntityGetState), null, null);
			base.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, ComponentHandleState>(this.OnBasicEntityHandleState), null, null);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003FA1 File Offset: 0x000021A1
		private void OnBasicEntityGetState(EntityUid uid, BasicEntityAmmoProviderComponent component, ref ComponentGetState args)
		{
			args.State = new BasicEntityAmmoProviderComponentState(component.Capacity, component.Count);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003FBC File Offset: 0x000021BC
		private void OnBasicEntityHandleState(EntityUid uid, BasicEntityAmmoProviderComponent component, ref ComponentHandleState args)
		{
			BasicEntityAmmoProviderComponentState state = args.Current as BasicEntityAmmoProviderComponentState;
			if (state != null)
			{
				component.Capacity = state.Capacity;
				component.Count = state.Count;
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00003FF0 File Offset: 0x000021F0
		private void OnBasicEntityInit(EntityUid uid, BasicEntityAmmoProviderComponent component, ComponentInit args)
		{
			int? count = component.Count;
			if (count == null)
			{
				component.Count = component.Capacity;
				base.Dirty(component, null);
			}
			this.UpdateBasicEntityAppearance(component);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00004028 File Offset: 0x00002228
		private void OnBasicEntityTakeAmmo(EntityUid uid, BasicEntityAmmoProviderComponent component, TakeAmmoEvent args)
		{
			for (int i = 0; i < args.Shots; i++)
			{
				int? count = component.Count;
				int num = 0;
				if (count.GetValueOrDefault() <= num & count != null)
				{
					return;
				}
				if (component.Count != null)
				{
					component.Count--;
				}
				EntityUid ent = base.Spawn(component.Proto, args.Coordinates);
				args.Ammo.Add(base.EnsureComp<AmmoComponent>(ent));
			}
			this.UpdateBasicEntityAppearance(component);
			base.Dirty(component, null);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000040E0 File Offset: 0x000022E0
		private void OnBasicEntityAmmoCount(EntityUid uid, BasicEntityAmmoProviderComponent component, ref GetAmmoCountEvent args)
		{
			args.Capacity = (component.Capacity ?? int.MaxValue);
			args.Count = (component.Count ?? int.MaxValue);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004138 File Offset: 0x00002338
		private void UpdateBasicEntityAppearance(BasicEntityAmmoProviderComponent component)
		{
			AppearanceComponent appearance;
			if (!this.Timing.IsFirstTimePredicted || !base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			SharedAppearanceSystem appearance2 = this.Appearance;
			EntityUid owner = appearance.Owner;
			Enum @enum = AmmoVisuals.HasAmmo;
			int? count = component.Count;
			int num = 0;
			appearance2.SetData(owner, @enum, !(count.GetValueOrDefault() == num & count != null), appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoCount, component.Count ?? int.MaxValue, appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoMax, component.Capacity ?? int.MaxValue, appearance);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00004214 File Offset: 0x00002414
		[NullableContext(2)]
		public bool UpdateBasicEntityAmmoCount(EntityUid uid, int count, BasicEntityAmmoProviderComponent component = null)
		{
			if (!base.Resolve<BasicEntityAmmoProviderComponent>(uid, ref component, true))
			{
				return false;
			}
			int? capacity = component.Capacity;
			if (count > capacity.GetValueOrDefault() & capacity != null)
			{
				return false;
			}
			component.Count = new int?(count);
			base.Dirty(component, null);
			this.UpdateBasicEntityAppearance(component);
			return true;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00004268 File Offset: 0x00002468
		protected virtual void InitializeBattery()
		{
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentGetState>(this.OnBatteryGetState), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentHandleState>(this.OnBatteryHandleState), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>(this.OnBatteryTakeAmmo), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>(this.OnBatteryAmmoCount), null, null);
			base.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, ExaminedEvent>(this.OnBatteryExamine), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentGetState>(this.OnBatteryGetState), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>(this.OnBatteryHandleState), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>(this.OnBatteryTakeAmmo), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>(this.OnBatteryAmmoCount), null, null);
			base.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>(this.OnBatteryExamine), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, ComponentInit>(this.OnTwoModeInit), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<TwoModeEnergyAmmoProviderComponent, ComponentGetState>(this.OnBatteryTwoModeGetState), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<TwoModeEnergyAmmoProviderComponent, ComponentHandleState>(this.OnBatteryTwoModeHandleState), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, TakeAmmoEvent>(this.OnBatteryTakeAmmo), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<TwoModeEnergyAmmoProviderComponent, GetAmmoCountEvent>(this.OnBatteryAmmoCount), null, null);
			base.SubscribeLocalEvent<TwoModeEnergyAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<TwoModeEnergyAmmoProviderComponent, ExaminedEvent>(this.OnBatteryExamine), null, null);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000043B8 File Offset: 0x000025B8
		private void OnTwoModeInit(EntityUid uid, TwoModeEnergyAmmoProviderComponent component, ComponentInit args)
		{
			AppearanceComponent appearance;
			if (!this.Timing.IsFirstTimePredicted || !base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.InStun, component.InStun, appearance);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00004408 File Offset: 0x00002608
		private void OnBatteryTwoModeHandleState(EntityUid uid, TwoModeEnergyAmmoProviderComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.TwoModeComponentState state = args.Current as SharedGunSystem.TwoModeComponentState;
			if (state == null)
			{
				return;
			}
			component.Shots = state.Shots;
			component.Capacity = state.MaxShots;
			component.FireCost = state.FireCost;
			component.CurrentMode = state.CurrentMode;
			component.InStun = state.InStun;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00004464 File Offset: 0x00002664
		private void OnBatteryTwoModeGetState(EntityUid uid, TwoModeEnergyAmmoProviderComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.TwoModeComponentState
			{
				Shots = component.Shots,
				MaxShots = component.Capacity,
				FireCost = component.FireCost,
				CurrentMode = component.CurrentMode,
				InStun = component.InStun
			};
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000044B8 File Offset: 0x000026B8
		private void OnBatteryHandleState(EntityUid uid, BatteryAmmoProviderComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.BatteryAmmoProviderComponentState state = args.Current as SharedGunSystem.BatteryAmmoProviderComponentState;
			if (state == null)
			{
				return;
			}
			component.Shots = state.Shots;
			component.Capacity = state.MaxShots;
			component.FireCost = state.FireCost;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000044F9 File Offset: 0x000026F9
		private void OnBatteryGetState(EntityUid uid, BatteryAmmoProviderComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.BatteryAmmoProviderComponentState
			{
				Shots = component.Shots,
				MaxShots = component.Capacity,
				FireCost = component.FireCost
			};
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000452C File Offset: 0x0000272C
		private void OnBatteryExamine(EntityUid uid, BatteryAmmoProviderComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("gun-battery-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("count", component.Shots)
			}));
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00004584 File Offset: 0x00002784
		private void OnBatteryTakeAmmo(EntityUid uid, BatteryAmmoProviderComponent component, TakeAmmoEvent args)
		{
			int shots = Math.Min(args.Shots, component.Shots);
			if (shots == 0)
			{
				return;
			}
			for (int i = 0; i < shots; i++)
			{
				args.Ammo.Add(this.GetShootable(component, args.Coordinates));
				component.Shots--;
			}
			this.TakeCharge(uid, component);
			this.UpdateBatteryAppearance(uid, component);
			base.Dirty(component, null);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000045F1 File Offset: 0x000027F1
		private void OnBatteryAmmoCount(EntityUid uid, BatteryAmmoProviderComponent component, ref GetAmmoCountEvent args)
		{
			args.Count = component.Shots;
			args.Capacity = component.Capacity;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000460B File Offset: 0x0000280B
		protected virtual void TakeCharge(EntityUid uid, BatteryAmmoProviderComponent component)
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004610 File Offset: 0x00002810
		protected void UpdateBatteryAppearance(EntityUid uid, BatteryAmmoProviderComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(uid, AmmoVisuals.HasAmmo, component.Shots != 0, appearance);
			this.Appearance.SetData(uid, AmmoVisuals.AmmoCount, component.Shots, appearance);
			this.Appearance.SetData(uid, AmmoVisuals.AmmoMax, component.Capacity, appearance);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00004688 File Offset: 0x00002888
		protected void UpdateTwoModeAppearance(EntityUid uid, TwoModeEnergyAmmoProviderComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			ItemComponent item;
			if (!base.TryComp<ItemComponent>(uid, ref item))
			{
				return;
			}
			if (component.InStun)
			{
				this._item.SetHeldPrefix(uid, null, item);
			}
			else
			{
				this._item.SetHeldPrefix(uid, "laser", item);
			}
			this.Appearance.SetData(uid, AmmoVisuals.InStun, component.InStun, appearance);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000046F8 File Offset: 0x000028F8
		private IShootable GetShootable(BatteryAmmoProviderComponent component, EntityCoordinates coordinates)
		{
			ProjectileBatteryAmmoProviderComponent proj = component as ProjectileBatteryAmmoProviderComponent;
			if (proj != null)
			{
				EntityUid ent = base.Spawn(proj.Prototype, coordinates);
				return base.EnsureComp<AmmoComponent>(ent);
			}
			HitscanBatteryAmmoProviderComponent hitscan = component as HitscanBatteryAmmoProviderComponent;
			if (hitscan != null)
			{
				return this.ProtoManager.Index<HitscanPrototype>(hitscan.Prototype);
			}
			TwoModeEnergyAmmoProviderComponent twoMode = component as TwoModeEnergyAmmoProviderComponent;
			if (twoMode == null)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (twoMode.CurrentMode == EnergyModes.Stun)
			{
				EntityUid projEnt = base.Spawn(twoMode.ProjectilePrototype, coordinates);
				return base.EnsureComp<AmmoComponent>(projEnt);
			}
			return this.ProtoManager.Index<HitscanPrototype>(twoMode.HitscanPrototype);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004784 File Offset: 0x00002984
		protected virtual void InitializeCartridge()
		{
			base.SubscribeLocalEvent<CartridgeAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<CartridgeAmmoComponent, ComponentGetState>(this.OnCartridgeGetState), null, null);
			base.SubscribeLocalEvent<CartridgeAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<CartridgeAmmoComponent, ComponentHandleState>(this.OnCartridgeHandleState), null, null);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000047B0 File Offset: 0x000029B0
		private void OnCartridgeHandleState(EntityUid uid, CartridgeAmmoComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.CartridgeAmmoComponentState state = args.Current as SharedGunSystem.CartridgeAmmoComponentState;
			if (state == null)
			{
				return;
			}
			component.Spent = state.Spent;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000047D9 File Offset: 0x000029D9
		private void OnCartridgeGetState(EntityUid uid, CartridgeAmmoComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.CartridgeAmmoComponentState
			{
				Spent = component.Spent
			};
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000047F4 File Offset: 0x000029F4
		protected virtual void InitializeChamberMagazine()
		{
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>(this.OnChamberMagazineTakeAmmo), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMagazineVerb), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnMagazineSlotChange), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnMagazineSlotChange), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, UseInHandEvent>(this.OnMagazineUse), null, null);
			base.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ExaminedEvent>(this.OnChamberMagazineExamine), null, null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000487C File Offset: 0x00002A7C
		private void OnChamberMagazineExamine(EntityUid uid, ChamberMagazineAmmoProviderComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			int count = this.GetChamberMagazineCountCapacity(component).Item1;
			args.PushMarkup(Loc.GetString("gun-magazine-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("count", count)
			}));
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000048E4 File Offset: 0x00002AE4
		private bool TryTakeChamberEntity(EntityUid uid, [NotNullWhen(true)] out EntityUid? entity)
		{
			IContainer container;
			if (this.Containers.TryGetContainer(uid, "gun_chamber", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null)
				{
					entity = slot.ContainedEntity;
					if (entity == null)
					{
						return false;
					}
					container.Remove(entity.Value, null, null, null, true, false, null, null);
					return true;
				}
			}
			entity = null;
			return false;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00004958 File Offset: 0x00002B58
		protected EntityUid? GetChamberEntity(EntityUid uid)
		{
			IContainer container;
			if (this.Containers.TryGetContainer(uid, "gun_chamber", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null)
				{
					return slot.ContainedEntity;
				}
			}
			return null;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004998 File Offset: 0x00002B98
		[NullableContext(0)]
		protected ValueTuple<int, int> GetChamberMagazineCountCapacity([Nullable(1)] ChamberMagazineAmmoProviderComponent component)
		{
			int num = (this.GetChamberEntity(component.Owner) != null) ? 1 : 0;
			ValueTuple<int, int> magazineCountCapacity = this.GetMagazineCountCapacity(component);
			int magCount = magazineCountCapacity.Item1;
			int magCapacity = magazineCountCapacity.Item2;
			return new ValueTuple<int, int>(num + magCount, magCapacity);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000049DC File Offset: 0x00002BDC
		private bool TryInsertChamber(EntityUid uid, EntityUid ammo)
		{
			IContainer container;
			if (this.Containers.TryGetContainer(uid, "gun_chamber", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null)
				{
					return slot.Insert(ammo, null, null, null, null, null);
				}
			}
			return false;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004A18 File Offset: 0x00002C18
		private void OnChamberMagazineTakeAmmo(EntityUid uid, ChamberMagazineAmmoProviderComponent component, TakeAmmoEvent args)
		{
			AppearanceComponent appearance;
			base.TryComp<AppearanceComponent>(uid, ref appearance);
			EntityUid? chamberEnt;
			if (this.TryTakeChamberEntity(uid, out chamberEnt))
			{
				args.Ammo.Add(base.EnsureComp<AmmoComponent>(chamberEnt.Value));
			}
			EntityUid? magEnt = this.GetMagazineEntity(uid);
			if (magEnt != null)
			{
				TakeAmmoEvent relayedArgs = new TakeAmmoEvent(args.Shots, new List<IShootable>(), args.Coordinates, args.User);
				base.RaiseLocalEvent<TakeAmmoEvent>(magEnt.Value, relayedArgs, false);
				if (relayedArgs.Ammo.Count > 0)
				{
					List<IShootable> ammo = relayedArgs.Ammo;
					EntityUid newChamberEnt = ((AmmoComponent)ammo[ammo.Count - 1]).Owner;
					this.TryInsertChamber(uid, newChamberEnt);
				}
				for (int i = 0; i < relayedArgs.Ammo.Count - 1; i++)
				{
					args.Ammo.Add(relayedArgs.Ammo[i]);
				}
				int count = (chamberEnt != null) ? 1 : 0;
				GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
				base.RaiseLocalEvent<GetAmmoCountEvent>(magEnt.Value, ref ammoEv, false);
				this.FinaliseMagazineTakeAmmo(uid, component, args, count + ammoEv.Count, 1 + ammoEv.Capacity, appearance);
				return;
			}
			this.Appearance.SetData(uid, AmmoVisuals.MagLoaded, false, appearance);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004B61 File Offset: 0x00002D61
		public void InitializeContainer()
		{
			base.SubscribeLocalEvent<ContainerAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ContainerAmmoProviderComponent, TakeAmmoEvent>(this.OnContainerTakeAmmo), null, null);
			base.SubscribeLocalEvent<ContainerAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ContainerAmmoProviderComponent, GetAmmoCountEvent>(this.OnContainerAmmoCount), null, null);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004B8C File Offset: 0x00002D8C
		private void OnContainerTakeAmmo(EntityUid uid, ContainerAmmoProviderComponent component, TakeAmmoEvent args)
		{
			IContainer container;
			if (!this._container.TryGetContainer(uid, component.Container, ref container, null))
			{
				return;
			}
			int i = 0;
			while (i < args.Shots && container.ContainedEntities.Any<EntityUid>())
			{
				EntityUid ent = container.ContainedEntities[0];
				if (this._netMan.IsServer)
				{
					container.Remove(ent, null, null, null, true, false, null, null);
				}
				args.Ammo.Add(base.EnsureComp<AmmoComponent>(ent));
				i++;
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00004C1C File Offset: 0x00002E1C
		private void OnContainerAmmoCount(EntityUid uid, ContainerAmmoProviderComponent component, ref GetAmmoCountEvent args)
		{
			IContainer container;
			if (!this._container.TryGetContainer(uid, component.Container, ref container, null))
			{
				args.Capacity = 0;
				args.Count = 0;
				return;
			}
			args.Capacity = int.MaxValue;
			args.Count = container.ContainedEntities.Count;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00004C6C File Offset: 0x00002E6C
		public override void Initialize()
		{
			this.Sawmill = Logger.GetSawmill("gun");
			this.Sawmill.Level = new LogLevel?(2);
			base.SubscribeLocalEvent<GunComponent, ComponentGetState>(new ComponentEventRefHandler<GunComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeAllEvent<RequestShootEvent>(new EntitySessionEventHandler<RequestShootEvent>(this.OnShootRequest), null, null);
			base.SubscribeAllEvent<RequestStopShootEvent>(new EntitySessionEventHandler<RequestStopShootEvent>(this.OnStopShootRequest), null, null);
			base.SubscribeLocalEvent<GunComponent, ComponentHandleState>(new ComponentEventRefHandler<GunComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<GunComponent, MeleeAttackAttemptEvent>(new ComponentEventRefHandler<GunComponent, MeleeAttackAttemptEvent>(this.OnGunMeleeAttempt), null, null);
			this.InitializeBallistic();
			this.InitializeBattery();
			this.InitializeCartridge();
			this.InitializeChamberMagazine();
			this.InitializeMagazine();
			this.InitializeRevolver();
			this.InitializeBasicEntity();
			this.InitializeContainer();
			base.SubscribeLocalEvent<GunComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<GunComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAltVerb), null, null);
			base.SubscribeLocalEvent<GunComponent, ExaminedEvent>(new ComponentEventHandler<GunComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<GunComponent, SharedGunSystem.CycleModeEvent>(new ComponentEventHandler<GunComponent, SharedGunSystem.CycleModeEvent>(this.OnCycleMode), null, null);
			base.SubscribeLocalEvent<GunComponent, ComponentInit>(new ComponentEventHandler<GunComponent, ComponentInit>(this.OnGunInit), null, null);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00004D7E File Offset: 0x00002F7E
		private void OnGunInit(EntityUid uid, GunComponent component, ComponentInit args)
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00004D80 File Offset: 0x00002F80
		private void OnGunMeleeAttempt(EntityUid uid, GunComponent component, ref MeleeAttackAttemptEvent args)
		{
			if (this.TagSystem.HasTag(args.User, "GunsDisabled"))
			{
				return;
			}
			args.Cancelled = true;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00004DA4 File Offset: 0x00002FA4
		private void OnShootRequest(RequestShootEvent msg, EntitySessionEventArgs args)
		{
			EntityUid? user = args.SenderSession.AttachedEntity;
			if (user == null)
			{
				return;
			}
			GunComponent gun = this.GetGun(user.Value);
			EntityUid? entityUid = (gun != null) ? new EntityUid?(gun.Owner) : null;
			EntityUid gun2 = msg.Gun;
			if (entityUid == null || (entityUid != null && entityUid.GetValueOrDefault() != gun2))
			{
				return;
			}
			gun.ShootCoordinates = new EntityCoordinates?(msg.Coordinates);
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Set shoot coordinates to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates?>(gun.ShootCoordinates);
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			this.AttemptShoot(user.Value, gun);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004E74 File Offset: 0x00003074
		private void OnStopShootRequest(RequestStopShootEvent ev, EntitySessionEventArgs args)
		{
			GunComponent gun;
			if (args.SenderSession.AttachedEntity == null || !base.TryComp<GunComponent>(ev.Gun, ref gun))
			{
				return;
			}
			if (this.GetGun(args.SenderSession.AttachedEntity.Value) != gun)
			{
				return;
			}
			this.StopShooting(gun);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00004ED0 File Offset: 0x000030D0
		private void OnGetState(EntityUid uid, GunComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.GunComponentState
			{
				FireRate = component.FireRate,
				CurrentAngle = component.CurrentAngle,
				MinAngle = component.MinAngle,
				MaxAngle = component.MaxAngle,
				NextFire = component.NextFire,
				ShotCounter = component.ShotCounter,
				SelectiveFire = component.SelectedMode,
				AvailableSelectiveFire = component.AvailableModes,
				SoundGunshot = component.SoundGunshot
			};
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00004F54 File Offset: 0x00003154
		private void OnHandleState(EntityUid uid, GunComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.GunComponentState state = args.Current as SharedGunSystem.GunComponentState;
			if (state == null)
			{
				return;
			}
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Handle state: setting shot count from ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(component.ShotCounter);
			defaultInterpolatedStringHandler.AppendLiteral(" to ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(state.ShotCounter);
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			component.FireRate = state.FireRate;
			component.CurrentAngle = state.CurrentAngle;
			component.MinAngle = state.MinAngle;
			component.MaxAngle = state.MaxAngle;
			component.NextFire = state.NextFire;
			component.ShotCounter = state.ShotCounter;
			component.SelectedMode = state.SelectiveFire;
			component.AvailableModes = state.AvailableSelectiveFire;
			component.SoundGunshot = state.SoundGunshot;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000502B File Offset: 0x0000322B
		public bool CanShoot(GunComponent component)
		{
			return !(component.NextFire > this.Timing.CurTime);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005048 File Offset: 0x00003248
		[NullableContext(2)]
		public GunComponent GetGun(EntityUid entity)
		{
			if (!this._combatMode.IsInCombatMode(new EntityUid?(entity), null))
			{
				return null;
			}
			SharedHandsComponent hands;
			GunComponent gun;
			if (this.EntityManager.TryGetComponent<SharedHandsComponent>(entity, ref hands))
			{
				EntityUid? activeHandEntity = hands.ActiveHandEntity;
				if (activeHandEntity != null)
				{
					EntityUid held = activeHandEntity.GetValueOrDefault();
					if (base.TryComp<GunComponent>(held, ref gun))
					{
						return gun;
					}
				}
			}
			if (base.TryComp<GunComponent>(entity, ref gun))
			{
				return gun;
			}
			return null;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000050B0 File Offset: 0x000032B0
		private void StopShooting(GunComponent gun)
		{
			if (gun.ShotCounter == 0)
			{
				return;
			}
			ISawmill sawmill = this.Sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Stopped shooting ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(gun.Owner));
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			gun.ShotCounter = 0;
			gun.ShootCoordinates = null;
			base.Dirty(gun, null);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000511C File Offset: 0x0000331C
		public void AttemptShoot(EntityUid user, GunComponent gun, EntityCoordinates toCoordinates)
		{
			gun.ShootCoordinates = new EntityCoordinates?(toCoordinates);
			this.AttemptShoot(user, gun);
			gun.ShotCounter = 0;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000513C File Offset: 0x0000333C
		private void AttemptShoot(EntityUid user, GunComponent gun)
		{
			if (gun.FireRate <= 0f)
			{
				return;
			}
			EntityCoordinates? toCoordinates = gun.ShootCoordinates;
			if (toCoordinates == null)
			{
				return;
			}
			if (this.TagSystem.HasTag(user, "GunsDisabled"))
			{
				this.Popup(Loc.GetString("gun-disabled"), new EntityUid?(user), new EntityUid?(user));
				return;
			}
			TimeSpan curTime = this.Timing.CurTime;
			if (gun.NextFire > curTime)
			{
				return;
			}
			if (gun.ShotCounter == 0 && gun.NextFire < curTime)
			{
				gun.NextFire = curTime;
			}
			int shots = 0;
			TimeSpan lastFire = gun.NextFire;
			TimeSpan fireRate = TimeSpan.FromSeconds((double)(1f / gun.FireRate));
			while (gun.NextFire <= curTime)
			{
				gun.NextFire += fireRate;
				shots++;
			}
			switch (gun.SelectedMode)
			{
			case SelectiveFire.SemiAuto:
				shots = Math.Min(shots, 1 - gun.ShotCounter);
				goto IL_14C;
			case SelectiveFire.Burst:
				shots = Math.Min(shots, 3 - gun.ShotCounter);
				goto IL_14C;
			case SelectiveFire.FullAuto:
				goto IL_14C;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler.AppendLiteral("No implemented shooting behavior for ");
			defaultInterpolatedStringHandler.AppendFormatted<SelectiveFire>(gun.SelectedMode);
			defaultInterpolatedStringHandler.AppendLiteral("!");
			throw new ArgumentOutOfRangeException(defaultInterpolatedStringHandler.ToStringAndClear());
			IL_14C:
			EntityCoordinates fromCoordinates = base.Transform(user).Coordinates;
			TakeAmmoEvent ev = new TakeAmmoEvent(shots, new List<IShootable>(), fromCoordinates, new EntityUid?(user));
			if (shots > 0)
			{
				base.RaiseLocalEvent<TakeAmmoEvent>(gun.Owner, ev, false);
			}
			this.UpdateAmmoCount(gun.Owner);
			gun.ShotCounter += shots;
			AttemptShootEvent attemptEv = new AttemptShootEvent(user, false);
			base.RaiseLocalEvent<AttemptShootEvent>(gun.Owner, ref attemptEv, false);
			if (ev.Ammo.Count > 0 && !attemptEv.Cancelled)
			{
				this.Shoot(gun, ev.Ammo, fromCoordinates, toCoordinates.Value, new EntityUid?(user));
				GunShotEvent shotEv = new GunShotEvent(user);
				base.RaiseLocalEvent<GunShotEvent>(gun.Owner, ref shotEv, false);
				PhysicsComponent userPhysics;
				if (base.TryComp<PhysicsComponent>(user, ref userPhysics) && this._gravity.IsWeightless(user, userPhysics, null))
				{
					this.CauseImpulse(fromCoordinates, toCoordinates.Value, userPhysics);
				}
				base.Dirty(gun, null);
				return;
			}
			if (shots > 0)
			{
				gun.NextFire = TimeSpan.FromSeconds(Math.Max(lastFire.TotalSeconds + 0.5, gun.NextFire.TotalSeconds));
				this.Audio.PlayPredicted(gun.SoundEmpty, gun.Owner, new EntityUid?(user), null);
				base.Dirty(gun, null);
				return;
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000053E0 File Offset: 0x000035E0
		public void Shoot(GunComponent gun, EntityUid ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid? user = null)
		{
			AmmoComponent shootable = base.EnsureComp<AmmoComponent>(ammo);
			this.Shoot(gun, new List<IShootable>(1)
			{
				shootable
			}, fromCoordinates, toCoordinates, user);
		}

		// Token: 0x060000DC RID: 220
		public abstract void Shoot(GunComponent gun, List<IShootable> ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid? user = null);

		// Token: 0x060000DD RID: 221 RVA: 0x0000540E File Offset: 0x0000360E
		public void Shoot(GunComponent gun, IShootable ammo, EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, EntityUid? user = null)
		{
			this.Shoot(gun, new List<IShootable>(1)
			{
				ammo
			}, fromCoordinates, toCoordinates, user);
		}

		// Token: 0x060000DE RID: 222
		protected abstract void Popup(string message, EntityUid? uid, EntityUid? user);

		// Token: 0x060000DF RID: 223 RVA: 0x00005429 File Offset: 0x00003629
		protected virtual void UpdateAmmoCount(EntityUid uid)
		{
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000542B File Offset: 0x0000362B
		protected void SetCartridgeSpent(CartridgeAmmoComponent cartridge, bool spent)
		{
			if (cartridge.Spent != spent)
			{
				base.Dirty(cartridge, null);
			}
			cartridge.Spent = spent;
			this.Appearance.SetData(cartridge.Owner, AmmoVisuals.Spent, spent, null);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005464 File Offset: 0x00003664
		protected void EjectCartridge(EntityUid entity, bool playSound = true)
		{
			Vector2 offsetPos = this.Random.NextVector2(0.4f);
			TransformComponent transformComponent = base.Transform(entity);
			EntityCoordinates coordinates = transformComponent.Coordinates.Offset(offsetPos);
			transformComponent.LocalRotation = this.Random.NextAngle();
			transformComponent.Coordinates = coordinates;
			CartridgeAmmoComponent cartridge;
			if (playSound && base.TryComp<CartridgeAmmoComponent>(entity, ref cartridge))
			{
				this.Audio.PlayPvs(cartridge.EjectSound, entity, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f)).WithVolume(-1f)));
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000054F8 File Offset: 0x000036F8
		protected void MuzzleFlash(EntityUid gun, AmmoComponent component, EntityUid? user = null)
		{
			string sprite = component.MuzzleFlash;
			if (sprite == null)
			{
				return;
			}
			string prototype = sprite;
			EntityUid? entityUid = user;
			MuzzleFlashEvent ev = new MuzzleFlashEvent(gun, prototype, entityUid != null && (entityUid == null || entityUid.GetValueOrDefault() == gun));
			this.CreateEffect(gun, ev, user);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000554C File Offset: 0x0000374C
		public void CauseImpulse(EntityCoordinates fromCoordinates, EntityCoordinates toCoordinates, PhysicsComponent userPhysics)
		{
			Vector2 fromMap = fromCoordinates.ToMapPos(this.EntityManager);
			Vector2 impulseVector = (toCoordinates.ToMapPos(this.EntityManager) - fromMap).Normalized * 5f;
			this.Physics.ApplyLinearImpulse(userPhysics.Owner, -impulseVector, null, userPhysics);
		}

		// Token: 0x060000E4 RID: 228
		protected abstract void CreateEffect(EntityUid uid, MuzzleFlashEvent message, EntityUid? user = null);

		// Token: 0x060000E5 RID: 229 RVA: 0x000055A8 File Offset: 0x000037A8
		private void OnExamine(EntityUid uid, GunComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange || !component.ShowExamineText)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("gun-selected-mode-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "cyan"),
				new ValueTuple<string, object>("mode", this.GetLocSelector(component.SelectedMode))
			}));
			args.PushMarkup(Loc.GetString("gun-fire-rate-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("fireRate", component.FireRate)
			}));
			TwoModeEnergyAmmoProviderComponent comp;
			if (!base.TryComp<TwoModeEnergyAmmoProviderComponent>(uid, ref comp))
			{
				return;
			}
			args.PushMarkup(Loc.GetString("gun-twomode-mode-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "red"),
				new ValueTuple<string, object>("mode", this.GetLocMode(comp.CurrentMode))
			}));
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000056AC File Offset: 0x000038AC
		private object GetLocMode(EnergyModes mode)
		{
			return Loc.GetString("gun-twomode-" + mode.ToString());
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000056CA File Offset: 0x000038CA
		private string GetLocSelector(SelectiveFire mode)
		{
			return Loc.GetString("gun-" + mode.ToString());
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000056E8 File Offset: 0x000038E8
		private void OnAltVerb(EntityUid uid, GunComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || component.SelectedMode == component.AvailableModes)
			{
				return;
			}
			SelectiveFire nextMode = this.GetNextMode(component);
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate()
				{
					this.SelectFire(component, nextMode, new EntityUid?(args.User));
				},
				Text = Loc.GetString("gun-selector-verb", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", this.GetLocSelector(nextMode))
				}),
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000057D0 File Offset: 0x000039D0
		private SelectiveFire GetNextMode(GunComponent component)
		{
			List<SelectiveFire> modes = new List<SelectiveFire>();
			foreach (SelectiveFire mode in Enum.GetValues<SelectiveFire>())
			{
				if ((mode & component.AvailableModes) != SelectiveFire.Invalid)
				{
					modes.Add(mode);
				}
			}
			int index = modes.IndexOf(component.SelectedMode);
			return modes[(index + 1) % modes.Count];
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000582C File Offset: 0x00003A2C
		private void SelectFire(GunComponent component, SelectiveFire fire, EntityUid? user = null)
		{
			if (component.SelectedMode == fire)
			{
				return;
			}
			component.SelectedMode = fire;
			TimeSpan curTime = this.Timing.CurTime;
			TimeSpan cooldown = TimeSpan.FromSeconds(0.30000001192092896);
			if (component.NextFire < curTime)
			{
				component.NextFire = curTime + cooldown;
			}
			else
			{
				component.NextFire += cooldown;
			}
			this.Audio.PlayPredicted(component.SoundModeToggle, component.Owner, user, null);
			this.Popup(Loc.GetString("gun-selected-mode", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mode", this.GetLocSelector(fire))
			}), new EntityUid?(component.Owner), user);
			base.Dirty(component, null);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000058F8 File Offset: 0x00003AF8
		public void CycleFire(GunComponent component, EntityUid? user = null)
		{
			if (component.SelectedMode == component.AvailableModes)
			{
				return;
			}
			SelectiveFire nextMode = this.GetNextMode(component);
			this.SelectFire(component, nextMode, user);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00005925 File Offset: 0x00003B25
		private void OnCycleMode(EntityUid uid, GunComponent component, SharedGunSystem.CycleModeEvent args)
		{
			this.SelectFire(component, args.Mode, new EntityUid?(args.Performer));
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00005940 File Offset: 0x00003B40
		protected virtual void InitializeMagazine()
		{
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, TakeAmmoEvent>(this.OnMagazineTakeAmmo), null, null);
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMagazineVerb), null, null);
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnMagazineSlotChange), null, null);
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnMagazineSlotChange), null, null);
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, UseInHandEvent>(this.OnMagazineUse), null, null);
			base.SubscribeLocalEvent<MagazineAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, ExaminedEvent>(this.OnMagazineExamine), null, null);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000059C8 File Offset: 0x00003BC8
		private void OnMagazineExamine(EntityUid uid, MagazineAmmoProviderComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			int count = this.GetMagazineCountCapacity(component).Item1;
			args.PushMarkup(Loc.GetString("gun-magazine-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", "yellow"),
				new ValueTuple<string, object>("count", count)
			}));
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00005A30 File Offset: 0x00003C30
		private void OnMagazineUse(EntityUid uid, MagazineAmmoProviderComponent component, UseInHandEvent args)
		{
			EntityUid? magEnt = this.GetMagazineEntity(uid);
			if (magEnt == null)
			{
				return;
			}
			base.RaiseLocalEvent<UseInHandEvent>(magEnt.Value, args, false);
			this.UpdateAmmoCount(uid);
			this.UpdateMagazineAppearance(component, magEnt.Value);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00005A74 File Offset: 0x00003C74
		private void OnMagazineVerb(EntityUid uid, MagazineAmmoProviderComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			EntityUid? magEnt = this.GetMagazineEntity(uid);
			if (magEnt != null)
			{
				base.RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(magEnt.Value, args, false);
				this.UpdateMagazineAppearance(component, magEnt.Value);
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00005AC0 File Offset: 0x00003CC0
		protected virtual void OnMagazineSlotChange(EntityUid uid, MagazineAmmoProviderComponent component, ContainerModifiedMessage args)
		{
			if ("gun_magazine" != args.Container.ID)
			{
				return;
			}
			this.UpdateAmmoCount(uid);
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this.Appearance.SetData(uid, AmmoVisuals.MagLoaded, this.GetMagazineEntity(uid) != null, appearance);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00005B20 File Offset: 0x00003D20
		[NullableContext(0)]
		protected ValueTuple<int, int> GetMagazineCountCapacity([Nullable(1)] MagazineAmmoProviderComponent component)
		{
			int count = 0;
			int capacity = 1;
			EntityUid? magEnt = this.GetMagazineEntity(component.Owner);
			if (magEnt != null)
			{
				GetAmmoCountEvent ev = default(GetAmmoCountEvent);
				base.RaiseLocalEvent<GetAmmoCountEvent>(magEnt.Value, ref ev, false);
				count += ev.Count;
				capacity += ev.Capacity;
			}
			return new ValueTuple<int, int>(count, capacity);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00005B78 File Offset: 0x00003D78
		protected EntityUid? GetMagazineEntity(EntityUid uid)
		{
			IContainer container;
			if (this.Containers.TryGetContainer(uid, "gun_magazine", ref container, null))
			{
				ContainerSlot slot = container as ContainerSlot;
				if (slot != null)
				{
					return slot.ContainedEntity;
				}
			}
			return null;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005BB8 File Offset: 0x00003DB8
		private void OnMagazineTakeAmmo(EntityUid uid, MagazineAmmoProviderComponent component, TakeAmmoEvent args)
		{
			EntityUid? magEntity = this.GetMagazineEntity(uid);
			AppearanceComponent appearance;
			base.TryComp<AppearanceComponent>(uid, ref appearance);
			if (magEntity == null)
			{
				this.Appearance.SetData(uid, AmmoVisuals.MagLoaded, false, appearance);
				return;
			}
			base.RaiseLocalEvent<TakeAmmoEvent>(magEntity.Value, args, false);
			GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
			base.RaiseLocalEvent<GetAmmoCountEvent>(magEntity.Value, ref ammoEv, false);
			this.FinaliseMagazineTakeAmmo(uid, component, args, ammoEv.Count, ammoEv.Capacity, appearance);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00005C38 File Offset: 0x00003E38
		private void FinaliseMagazineTakeAmmo(EntityUid uid, MagazineAmmoProviderComponent component, TakeAmmoEvent args, int count, int capacity, [Nullable(2)] AppearanceComponent appearance)
		{
			if (component.AutoEject && args.Ammo.Count == 0)
			{
				this.EjectMagazine(component);
				this.Audio.PlayPredicted(component.SoundAutoEject, uid, args.User, null);
			}
			this.UpdateMagazineAppearance(appearance, true, count, capacity);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00005C90 File Offset: 0x00003E90
		private void UpdateMagazineAppearance(MagazineAmmoProviderComponent component, EntityUid magEnt)
		{
			AppearanceComponent appearance;
			base.TryComp<AppearanceComponent>(component.Owner, ref appearance);
			int count = 0;
			int capacity = 0;
			ChamberMagazineAmmoProviderComponent chamber = component as ChamberMagazineAmmoProviderComponent;
			if (chamber != null)
			{
				count = ((this.GetChamberEntity(chamber.Owner) != null) ? 1 : 0);
				capacity = 1;
			}
			AppearanceComponent magAppearance;
			if (base.TryComp<AppearanceComponent>(magEnt, ref magAppearance))
			{
				int addCount;
				this.Appearance.TryGetData<int>(magEnt, AmmoVisuals.AmmoCount, ref addCount, magAppearance);
				int addCapacity;
				this.Appearance.TryGetData<int>(magEnt, AmmoVisuals.AmmoMax, ref addCapacity, magAppearance);
				count += addCount;
				capacity += addCapacity;
			}
			this.UpdateMagazineAppearance(appearance, true, count, capacity);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00005D28 File Offset: 0x00003F28
		[NullableContext(2)]
		private void UpdateMagazineAppearance(AppearanceComponent appearance, bool magLoaded, int count, int capacity)
		{
			if (appearance == null)
			{
				return;
			}
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.MagLoaded, magLoaded, appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.HasAmmo, count != 0, appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoCount, count, appearance);
			this.Appearance.SetData(appearance.Owner, AmmoVisuals.AmmoMax, capacity, appearance);
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00005DB8 File Offset: 0x00003FB8
		private void EjectMagazine(MagazineAmmoProviderComponent component)
		{
			if (this.GetMagazineEntity(component.Owner) == null)
			{
				return;
			}
			EntityUid? a;
			this._slots.TryEject(component.Owner, "gun_magazine", null, out a, null, true);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00005E00 File Offset: 0x00004000
		protected virtual void InitializeRevolver()
		{
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentGetState>(this.OnRevolverGetState), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentHandleState>(this.OnRevolverHandleState), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<RevolverAmmoProviderComponent, ComponentInit>(this.OnRevolverInit), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, TakeAmmoEvent>(this.OnRevolverTakeAmmo), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnRevolverVerbs), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, InteractUsingEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, InteractUsingEvent>(this.OnRevolverInteractUsing), null, null);
			base.SubscribeLocalEvent<RevolverAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, GetAmmoCountEvent>(this.OnRevolverGetAmmoCount), null, null);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005E99 File Offset: 0x00004099
		private void OnRevolverGetAmmoCount(EntityUid uid, RevolverAmmoProviderComponent component, ref GetAmmoCountEvent args)
		{
			args.Count += this.GetRevolverCount(component);
			args.Capacity += component.Capacity;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00005EBC File Offset: 0x000040BC
		private void OnRevolverInteractUsing(EntityUid uid, RevolverAmmoProviderComponent component, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			if (this.TryRevolverInsert(component, args.Used, new EntityUid?(args.User)))
			{
				args.Handled = true;
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00005EE8 File Offset: 0x000040E8
		private void OnRevolverGetState(EntityUid uid, RevolverAmmoProviderComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGunSystem.RevolverAmmoProviderComponentState
			{
				CurrentIndex = component.CurrentIndex,
				AmmoSlots = component.AmmoSlots,
				Chambers = component.Chambers
			};
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00005F1C File Offset: 0x0000411C
		private void OnRevolverHandleState(EntityUid uid, RevolverAmmoProviderComponent component, ref ComponentHandleState args)
		{
			SharedGunSystem.RevolverAmmoProviderComponentState state = args.Current as SharedGunSystem.RevolverAmmoProviderComponentState;
			if (state == null)
			{
				return;
			}
			int oldIndex = component.CurrentIndex;
			component.CurrentIndex = state.CurrentIndex;
			component.Chambers = new bool?[state.Chambers.Length];
			for (int i = 0; i < component.AmmoSlots.Count; i++)
			{
				component.AmmoSlots[i] = state.AmmoSlots[i];
				component.Chambers[i] = state.Chambers[i];
			}
			if (this.Timing.IsFirstTimePredicted && oldIndex != state.CurrentIndex)
			{
				this.UpdateAmmoCount(uid);
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00005FC4 File Offset: 0x000041C4
		public bool TryRevolverInsert(RevolverAmmoProviderComponent component, EntityUid uid, EntityUid? user)
		{
			EntityWhitelist whitelist = component.Whitelist;
			if (whitelist != null && !whitelist.IsValid(uid, this.EntityManager))
			{
				return false;
			}
			if (!this.EntityManager.HasComponent<SpeedLoaderComponent>(uid))
			{
				for (int i = 0; i < component.Capacity; i++)
				{
					int index = (component.CurrentIndex + i) % component.Capacity;
					if (component.AmmoSlots[index] == null && component.Chambers[index] == null)
					{
						component.AmmoSlots[index] = new EntityUid?(uid);
						component.AmmoContainer.Insert(uid, null, null, null, null, null);
						this.Audio.PlayPredicted(component.SoundInsert, component.Owner, user, null);
						this.Popup(Loc.GetString("gun-revolver-insert"), new EntityUid?(component.Owner), user);
						this.UpdateRevolverAppearance(component);
						this.UpdateAmmoCount(uid);
						base.Dirty(component, null);
						return true;
					}
				}
				this.Popup(Loc.GetString("gun-revolver-full"), new EntityUid?(component.Owner), user);
				return false;
			}
			int freeSlots = 0;
			for (int j = 0; j < component.Capacity; j++)
			{
				if (component.AmmoSlots[j] == null && component.Chambers[j] == null)
				{
					freeSlots++;
				}
			}
			if (freeSlots == 0)
			{
				this.Popup(Loc.GetString("gun-revolver-full"), new EntityUid?(component.Owner), user);
				return false;
			}
			TransformComponent xform = base.GetEntityQuery<TransformComponent>().GetComponent(uid);
			List<IShootable> ammo = new List<IShootable>(freeSlots);
			TakeAmmoEvent ev = new TakeAmmoEvent(freeSlots, ammo, xform.Coordinates, user);
			base.RaiseLocalEvent<TakeAmmoEvent>(uid, ev, false);
			if (ev.Ammo.Count == 0)
			{
				this.Popup(Loc.GetString("gun-speedloader-empty"), new EntityUid?(component.Owner), user);
				return false;
			}
			for (int k = Math.Min(ev.Ammo.Count - 1, component.Capacity - 1); k >= 0; k--)
			{
				int index2 = (component.CurrentIndex + k) % component.Capacity;
				if (component.AmmoSlots[index2] == null && component.Chambers[index2] == null)
				{
					IShootable shootable = ev.Ammo.Last<IShootable>();
					ev.Ammo.RemoveAt(ev.Ammo.Count - 1);
					AmmoComponent ammoComp = shootable as AmmoComponent;
					if (ammoComp == null)
					{
						this.Sawmill.Error("Tried to load hitscan into a revolver which is unsupported");
					}
					else
					{
						component.AmmoSlots[index2] = new EntityUid?(ammoComp.Owner);
						component.AmmoContainer.Insert(ammoComp.Owner, this.EntityManager, null, null, null, null);
						if (ev.Ammo.Count == 0)
						{
							break;
						}
					}
				}
			}
			this.UpdateRevolverAppearance(component);
			this.UpdateAmmoCount(uid);
			base.Dirty(component, null);
			this.Audio.PlayPredicted(component.SoundInsert, component.Owner, user, null);
			this.Popup(Loc.GetString("gun-revolver-insert"), new EntityUid?(component.Owner), user);
			return true;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006310 File Offset: 0x00004510
		private void OnRevolverVerbs(EntityUid uid, RevolverAmmoProviderComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Text = Loc.GetString("gun-revolver-empty"),
				Disabled = !this.AnyRevolverCartridges(component),
				Act = delegate()
				{
					this.EmptyRevolver(component, new EntityUid?(args.User));
				},
				Priority = 1
			});
			args.Verbs.Add(new AlternativeVerb
			{
				Text = Loc.GetString("gun-revolver-spin"),
				Act = delegate()
				{
					this.SpinRevolver(component, new EntityUid?(args.User));
				}
			});
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000063EC File Offset: 0x000045EC
		private bool AnyRevolverCartridges(RevolverAmmoProviderComponent component)
		{
			for (int i = 0; i < component.Capacity; i++)
			{
				if (component.Chambers[i] != null || component.AmmoSlots[i] != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006438 File Offset: 0x00004638
		private int GetRevolverCount(RevolverAmmoProviderComponent component)
		{
			int count = 0;
			for (int i = 0; i < component.Capacity; i++)
			{
				if (component.Chambers[i] != null || component.AmmoSlots[i] != null)
				{
					count++;
				}
			}
			return count;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006488 File Offset: 0x00004688
		private int GetRevolverUnspentCount(RevolverAmmoProviderComponent component)
		{
			int count = 0;
			for (int i = 0; i < component.Capacity; i++)
			{
				bool? flag = component.Chambers[i];
				bool flag2 = true;
				if (flag.GetValueOrDefault() == flag2 & flag != null)
				{
					count++;
				}
				else
				{
					EntityUid? ammo = component.AmmoSlots[i];
					CartridgeAmmoComponent cartridge;
					if (base.TryComp<CartridgeAmmoComponent>(ammo, ref cartridge) && !cartridge.Spent)
					{
						count++;
					}
				}
			}
			return count;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000064FC File Offset: 0x000046FC
		public void EmptyRevolver(RevolverAmmoProviderComponent component, EntityUid? user = null)
		{
			MapCoordinates mapCoordinates = base.Transform(component.Owner).MapPosition;
			bool anyEmpty = false;
			for (int i = 0; i < component.Capacity; i++)
			{
				bool? chamber = component.Chambers[i];
				EntityUid? slot = component.AmmoSlots[i];
				if (slot == null)
				{
					if (chamber != null)
					{
						if (!this._netManager.IsClient)
						{
							EntityUid uid = base.Spawn(component.FillPrototype, mapCoordinates);
							CartridgeAmmoComponent cartridge;
							if (base.TryComp<CartridgeAmmoComponent>(uid, ref cartridge))
							{
								this.SetCartridgeSpent(cartridge, !chamber.Value);
							}
							this.EjectCartridge(uid, true);
						}
						component.Chambers[i] = null;
						anyEmpty = true;
					}
				}
				else
				{
					component.AmmoSlots[i] = null;
					component.AmmoContainer.Remove(slot.Value, null, null, null, true, false, null, null);
					if (!this._netManager.IsClient)
					{
						this.EjectCartridge(slot.Value, true);
					}
					anyEmpty = true;
				}
			}
			if (anyEmpty)
			{
				this.Audio.PlayPredicted(component.SoundEject, component.Owner, user, null);
				this.UpdateAmmoCount(component.Owner);
				this.UpdateRevolverAppearance(component);
				base.Dirty(component, null);
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006660 File Offset: 0x00004860
		private void UpdateRevolverAppearance(RevolverAmmoProviderComponent component)
		{
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				return;
			}
			int count = this.GetRevolverCount(component);
			this.Appearance.SetData(component.Owner, AmmoVisuals.HasAmmo, count != 0, appearance);
			this.Appearance.SetData(component.Owner, AmmoVisuals.AmmoCount, count, appearance);
			this.Appearance.SetData(component.Owner, AmmoVisuals.AmmoMax, component.Capacity, appearance);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000066E8 File Offset: 0x000048E8
		protected virtual void SpinRevolver(RevolverAmmoProviderComponent component, EntityUid? user = null)
		{
			this.Audio.PlayPredicted(component.SoundSpin, component.Owner, user, null);
			this.Popup(Loc.GetString("gun-revolver-spun"), new EntityUid?(component.Owner), user);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006734 File Offset: 0x00004934
		private void OnRevolverTakeAmmo(EntityUid uid, RevolverAmmoProviderComponent component, TakeAmmoEvent args)
		{
			int currentIndex = component.CurrentIndex;
			this.Cycle(component, args.Shots);
			for (int i = 0; i < args.Shots; i++)
			{
				int index = (currentIndex + i) % component.Capacity;
				bool? chamber = component.Chambers[index];
				if (chamber != null)
				{
					bool? flag = chamber;
					bool flag2 = true;
					if (flag.GetValueOrDefault() == flag2 & flag != null)
					{
						EntityUid ent = base.Spawn(component.FillPrototype, args.Coordinates);
						CartridgeAmmoComponent cartridge;
						if (base.TryComp<CartridgeAmmoComponent>(ent, ref cartridge))
						{
							component.Chambers[index] = new bool?(false);
							this.SetCartridgeSpent(cartridge, true);
							args.Ammo.Add(base.EnsureComp<AmmoComponent>(base.Spawn(cartridge.Prototype, args.Coordinates)));
							base.Del(ent);
						}
						else
						{
							component.Chambers[i] = null;
							args.Ammo.Add(base.EnsureComp<AmmoComponent>(ent));
						}
					}
				}
				else if (component.AmmoSlots[index] != null)
				{
					EntityUid? ent2 = component.AmmoSlots[index];
					CartridgeAmmoComponent cartridge2;
					if (base.TryComp<CartridgeAmmoComponent>(ent2, ref cartridge2))
					{
						if (!cartridge2.Spent)
						{
							this.SetCartridgeSpent(cartridge2, true);
							args.Ammo.Add(base.EnsureComp<AmmoComponent>(base.Spawn(cartridge2.Prototype, args.Coordinates)));
						}
					}
					else
					{
						component.AmmoContainer.Remove(ent2.Value, null, null, null, true, false, null, null);
						component.AmmoSlots[index] = null;
						args.Ammo.Add(base.EnsureComp<AmmoComponent>(ent2.Value));
						base.Transform(ent2.Value).Coordinates = args.Coordinates;
					}
				}
			}
			this.UpdateRevolverAppearance(component);
			base.Dirty(component, null);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00006934 File Offset: 0x00004B34
		private void Cycle(RevolverAmmoProviderComponent component, int count = 1)
		{
			component.CurrentIndex = (component.CurrentIndex + count) % component.Capacity;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000694C File Offset: 0x00004B4C
		private void OnRevolverInit(EntityUid uid, RevolverAmmoProviderComponent component, ComponentInit args)
		{
			component.AmmoContainer = this.Containers.EnsureContainer<Container>(uid, "revolver-ammo", null);
			component.AmmoSlots.EnsureCapacity(component.Capacity);
			int remainder = component.Capacity - component.AmmoSlots.Count;
			for (int i = 0; i < remainder; i++)
			{
				component.AmmoSlots.Add(null);
			}
			component.Chambers = new bool?[component.Capacity];
			if (component.FillPrototype != null)
			{
				for (int j = 0; j < component.Capacity; j++)
				{
					if (component.AmmoSlots[j] != null)
					{
						component.Chambers[j] = null;
					}
					else
					{
						component.Chambers[j] = new bool?(true);
					}
				}
			}
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006A24 File Offset: 0x00004C24
		[CompilerGenerated]
		private void <OnBallisticAfterInteract>g__SimulateInsertAmmo|3_0(EntityUid ammo, EntityUid ammoProvider, EntityCoordinates coordinates, ref SharedGunSystem.<>c__DisplayClass3_0 A_4)
		{
			InteractUsingEvent evInsert = new InteractUsingEvent(A_4.args.User, ammo, ammoProvider, coordinates);
			base.RaiseLocalEvent<InteractUsingEvent>(ammoProvider, evInsert, false);
		}

		// Token: 0x040000CA RID: 202
		[Dependency]
		private readonly SharedItemSystem _item;

		// Token: 0x040000CB RID: 203
		protected const string ChamberSlot = "gun_chamber";

		// Token: 0x040000CC RID: 204
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x040000CD RID: 205
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x040000CE RID: 206
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x040000CF RID: 207
		[Dependency]
		protected readonly IMapManager MapManager;

		// Token: 0x040000D0 RID: 208
		[Dependency]
		private readonly INetManager _netManager;

		// Token: 0x040000D1 RID: 209
		[Dependency]
		protected readonly IPrototypeManager ProtoManager;

		// Token: 0x040000D2 RID: 210
		[Dependency]
		protected readonly IRobustRandom Random;

		// Token: 0x040000D3 RID: 211
		[Dependency]
		protected readonly ISharedAdminLogManager Logs;

		// Token: 0x040000D4 RID: 212
		[Dependency]
		protected readonly DamageableSystem Damageable;

		// Token: 0x040000D5 RID: 213
		[Dependency]
		protected readonly ExamineSystemShared Examine;

		// Token: 0x040000D6 RID: 214
		[Dependency]
		private readonly ItemSlotsSystem _slots;

		// Token: 0x040000D7 RID: 215
		[Dependency]
		protected readonly SharedActionsSystem Actions;

		// Token: 0x040000D8 RID: 216
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x040000D9 RID: 217
		[Dependency]
		private readonly SharedCombatModeSystem _combatMode;

		// Token: 0x040000DA RID: 218
		[Dependency]
		protected readonly SharedContainerSystem Containers;

		// Token: 0x040000DB RID: 219
		[Dependency]
		protected readonly SharedPhysicsSystem Physics;

		// Token: 0x040000DC RID: 220
		[Dependency]
		protected readonly SharedPopupSystem PopupSystem;

		// Token: 0x040000DD RID: 221
		[Dependency]
		protected readonly ThrowingSystem ThrowingSystem;

		// Token: 0x040000DE RID: 222
		[Dependency]
		protected readonly TagSystem TagSystem;

		// Token: 0x040000DF RID: 223
		[Dependency]
		protected readonly SharedAudioSystem Audio;

		// Token: 0x040000E0 RID: 224
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x040000E1 RID: 225
		[Dependency]
		protected readonly SharedProjectileSystem Projectiles;

		// Token: 0x040000E2 RID: 226
		protected ISawmill Sawmill;

		// Token: 0x040000E3 RID: 227
		private const float InteractNextFire = 0.3f;

		// Token: 0x040000E4 RID: 228
		private const double SafetyNextFire = 0.5;

		// Token: 0x040000E5 RID: 229
		private const float EjectOffset = 0.4f;

		// Token: 0x040000E6 RID: 230
		protected const string AmmoExamineColor = "yellow";

		// Token: 0x040000E7 RID: 231
		protected const string FireRateExamineColor = "yellow";

		// Token: 0x040000E8 RID: 232
		protected const string ModeExamineColor = "cyan";

		// Token: 0x040000E9 RID: 233
		protected const string TwoModeExamineColor = "red";

		// Token: 0x040000EA RID: 234
		protected const string MagazineSlot = "gun_magazine";

		// Token: 0x040000EB RID: 235
		protected const string RevolverContainer = "revolver-ammo";

		// Token: 0x02000782 RID: 1922
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class BallisticAmmoProviderComponentState : ComponentState
		{
			// Token: 0x0400176D RID: 5997
			public int UnspawnedCount;

			// Token: 0x0400176E RID: 5998
			[Nullable(1)]
			public List<EntityUid> Entities;

			// Token: 0x0400176F RID: 5999
			public bool Cycled;
		}

		// Token: 0x02000783 RID: 1923
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class BatteryAmmoProviderComponentState : ComponentState
		{
			// Token: 0x04001770 RID: 6000
			public int Shots;

			// Token: 0x04001771 RID: 6001
			public int MaxShots;

			// Token: 0x04001772 RID: 6002
			public float FireCost;
		}

		// Token: 0x02000784 RID: 1924
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class TwoModeComponentState : ComponentState
		{
			// Token: 0x170004EE RID: 1262
			// (get) Token: 0x060017B2 RID: 6066 RVA: 0x0004CFDB File Offset: 0x0004B1DB
			// (set) Token: 0x060017B3 RID: 6067 RVA: 0x0004CFE3 File Offset: 0x0004B1E3
			public EnergyModes CurrentMode { get; set; }

			// Token: 0x04001774 RID: 6004
			public int Shots;

			// Token: 0x04001775 RID: 6005
			public int MaxShots;

			// Token: 0x04001776 RID: 6006
			public float FireCost;

			// Token: 0x04001777 RID: 6007
			public bool InStun;
		}

		// Token: 0x02000785 RID: 1925
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class CartridgeAmmoComponentState : ComponentState
		{
			// Token: 0x04001778 RID: 6008
			public bool Spent;
		}

		// Token: 0x02000786 RID: 1926
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class GunComponentState : ComponentState
		{
			// Token: 0x04001779 RID: 6009
			public Angle CurrentAngle;

			// Token: 0x0400177A RID: 6010
			public Angle MinAngle;

			// Token: 0x0400177B RID: 6011
			public Angle MaxAngle;

			// Token: 0x0400177C RID: 6012
			public TimeSpan NextFire;

			// Token: 0x0400177D RID: 6013
			public float FireRate;

			// Token: 0x0400177E RID: 6014
			public int ShotCounter;

			// Token: 0x0400177F RID: 6015
			public SelectiveFire SelectiveFire;

			// Token: 0x04001780 RID: 6016
			public SelectiveFire AvailableSelectiveFire;

			// Token: 0x04001781 RID: 6017
			[Nullable(2)]
			public SoundSpecifier SoundGunshot;
		}

		// Token: 0x02000787 RID: 1927
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		public sealed class HitscanEvent : EntityEventArgs
		{
			// Token: 0x04001782 RID: 6018
			[TupleElementNames(new string[]
			{
				"coordinates",
				"angle",
				"Sprite",
				"Distance"
			})]
			[Nullable(new byte[]
			{
				1,
				0,
				1
			})]
			public List<ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>> Sprites = new List<ValueTuple<EntityCoordinates, Angle, SpriteSpecifier, float>>();
		}

		// Token: 0x02000788 RID: 1928
		[NullableContext(0)]
		private sealed class CycleModeEvent : InstantActionEvent
		{
			// Token: 0x04001783 RID: 6019
			public SelectiveFire Mode;
		}

		// Token: 0x02000789 RID: 1929
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class RevolverAmmoProviderComponentState : ComponentState
		{
			// Token: 0x04001784 RID: 6020
			public int CurrentIndex;

			// Token: 0x04001785 RID: 6021
			public List<EntityUid?> AmmoSlots;

			// Token: 0x04001786 RID: 6022
			public bool?[] Chambers;
		}

		// Token: 0x0200078A RID: 1930
		[NullableContext(0)]
		public sealed class RevolverSpinEvent : EntityEventArgs
		{
		}
	}
}
