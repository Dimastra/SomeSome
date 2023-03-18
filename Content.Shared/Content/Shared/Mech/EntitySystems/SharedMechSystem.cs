using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Destructible;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Mech.EntitySystems
{
	// Token: 0x02000327 RID: 807
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMechSystem : EntitySystem
	{
		// Token: 0x06000938 RID: 2360 RVA: 0x0001EB20 File Offset: 0x0001CD20
		public override void Initialize()
		{
			base.SubscribeLocalEvent<SharedMechComponent, ComponentGetState>(new ComponentEventRefHandler<SharedMechComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, ComponentHandleState>(new ComponentEventRefHandler<SharedMechComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, ComponentGetState>(new ComponentEventRefHandler<MechPilotComponent, ComponentGetState>(this.OnPilotGetState), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, ComponentHandleState>(new ComponentEventRefHandler<MechPilotComponent, ComponentHandleState>(this.OnPilotHandleState), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, MechToggleEquipmentEvent>(new ComponentEventHandler<SharedMechComponent, MechToggleEquipmentEvent>(this.OnToggleEquipmentAction), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, MechEjectPilotEvent>(new ComponentEventHandler<SharedMechComponent, MechEjectPilotEvent>(this.OnEjectPilotEvent), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, InteractNoHandEvent>(new ComponentEventHandler<SharedMechComponent, InteractNoHandEvent>(this.RelayInteractionEvent), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, ComponentStartup>(new ComponentEventHandler<SharedMechComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, DestructionEventArgs>(new ComponentEventHandler<SharedMechComponent, DestructionEventArgs>(this.OnDestruction), null, null);
			base.SubscribeLocalEvent<SharedMechComponent, GetAdditionalAccessEvent>(new ComponentEventRefHandler<SharedMechComponent, GetAdditionalAccessEvent>(this.OnGetAdditionalAccess), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, GetMeleeWeaponEvent>(new ComponentEventHandler<MechPilotComponent, GetMeleeWeaponEvent>(this.OnGetMeleeWeapon), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, CanAttackFromContainerEvent>(new ComponentEventHandler<MechPilotComponent, CanAttackFromContainerEvent>(this.OnCanAttackFromContainer), null, null);
			base.SubscribeLocalEvent<MechPilotComponent, AttackAttemptEvent>(new ComponentEventHandler<MechPilotComponent, AttackAttemptEvent>(this.OnAttackAttempt), null, null);
		}

		// Token: 0x06000939 RID: 2361 RVA: 0x0001EC34 File Offset: 0x0001CE34
		private void OnGetState(EntityUid uid, SharedMechComponent component, ref ComponentGetState args)
		{
			args.State = new MechComponentState
			{
				Integrity = component.Integrity,
				MaxIntegrity = component.MaxIntegrity,
				Energy = component.Energy,
				MaxEnergy = component.MaxEnergy,
				CurrentSelectedEquipment = component.CurrentSelectedEquipment,
				Broken = component.Broken
			};
		}

		// Token: 0x0600093A RID: 2362 RVA: 0x0001EC94 File Offset: 0x0001CE94
		private void OnHandleState(EntityUid uid, SharedMechComponent component, ref ComponentHandleState args)
		{
			MechComponentState state = args.Current as MechComponentState;
			if (state == null)
			{
				return;
			}
			component.Integrity = state.Integrity;
			component.MaxIntegrity = state.MaxIntegrity;
			component.Energy = state.Energy;
			component.MaxEnergy = state.MaxEnergy;
			component.CurrentSelectedEquipment = state.CurrentSelectedEquipment;
			component.Broken = state.Broken;
		}

		// Token: 0x0600093B RID: 2363 RVA: 0x0001ECF9 File Offset: 0x0001CEF9
		private void OnPilotGetState(EntityUid uid, MechPilotComponent component, ref ComponentGetState args)
		{
			args.State = new MechPilotComponentState
			{
				Mech = component.Mech
			};
		}

		// Token: 0x0600093C RID: 2364 RVA: 0x0001ED14 File Offset: 0x0001CF14
		private void OnPilotHandleState(EntityUid uid, MechPilotComponent component, ref ComponentHandleState args)
		{
			MechPilotComponentState state = args.Current as MechPilotComponentState;
			if (state == null)
			{
				return;
			}
			component.Mech = state.Mech;
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0001ED3D File Offset: 0x0001CF3D
		private void OnToggleEquipmentAction(EntityUid uid, SharedMechComponent component, MechToggleEquipmentEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.CycleEquipment(uid, null);
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x0001ED57 File Offset: 0x0001CF57
		private void OnEjectPilotEvent(EntityUid uid, SharedMechComponent component, MechEjectPilotEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			this.TryEject(uid, component);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x0001ED74 File Offset: 0x0001CF74
		private void RelayInteractionEvent(EntityUid uid, SharedMechComponent component, InteractNoHandEvent args)
		{
			if (component.PilotSlot.ContainedEntity == null)
			{
				return;
			}
			if (!this._timing.IsFirstTimePredicted)
			{
				return;
			}
			if (component.CurrentSelectedEquipment != null)
			{
				base.RaiseLocalEvent<InteractNoHandEvent>(component.CurrentSelectedEquipment.Value, args, false);
			}
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x0001EDC8 File Offset: 0x0001CFC8
		private void OnStartup(EntityUid uid, SharedMechComponent component, ComponentStartup args)
		{
			component.PilotSlot = this._container.EnsureContainer<ContainerSlot>(uid, component.PilotSlotId, null);
			component.EquipmentContainer = this._container.EnsureContainer<Container>(uid, component.EquipmentContainerId, null);
			component.BatterySlot = this._container.EnsureContainer<ContainerSlot>(uid, component.BatterySlotId, null);
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0001EE29 File Offset: 0x0001D029
		private void OnDestruction(EntityUid uid, SharedMechComponent component, DestructionEventArgs args)
		{
			this.BreakMech(uid, component);
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x0001EE34 File Offset: 0x0001D034
		private void OnGetAdditionalAccess(EntityUid uid, SharedMechComponent component, ref GetAdditionalAccessEvent args)
		{
			EntityUid? pilot = component.PilotSlot.ContainedEntity;
			if (pilot == null)
			{
				return;
			}
			args.Entities.Add(pilot.Value);
			HashSet<EntityUid> items;
			this._access.FindAccessItemsInventory(pilot.Value, out items);
			args.Entities = args.Entities.Union(items).ToHashSet<EntityUid>();
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0001EE98 File Offset: 0x0001D098
		[NullableContext(2)]
		private void SetupUser(EntityUid mech, EntityUid pilot, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(mech, ref component, true))
			{
				return;
			}
			MechPilotComponent rider = base.EnsureComp<MechPilotComponent>(pilot);
			RelayInputMoverComponent relay = base.EnsureComp<RelayInputMoverComponent>(pilot);
			InteractionRelayComponent irelay = base.EnsureComp<InteractionRelayComponent>(pilot);
			this._mover.SetRelay(pilot, mech, relay);
			this._interaction.SetRelay(pilot, new EntityUid?(mech), irelay);
			rider.Mech = mech;
			base.Dirty(rider, null);
			this._actions.AddAction(pilot, new InstantAction(this._prototype.Index<InstantActionPrototype>(component.MechCycleAction)), new EntityUid?(mech), null, true);
			this._actions.AddAction(pilot, new InstantAction(this._prototype.Index<InstantActionPrototype>(component.MechUiAction)), new EntityUid?(mech), null, true);
			this._actions.AddAction(pilot, new InstantAction(this._prototype.Index<InstantActionPrototype>(component.MechEjectAction)), new EntityUid?(mech), null, true);
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x0001EF78 File Offset: 0x0001D178
		private void RemoveUser(EntityUid mech, EntityUid pilot)
		{
			if (!base.RemComp<MechPilotComponent>(pilot))
			{
				return;
			}
			base.RemComp<RelayInputMoverComponent>(pilot);
			base.RemComp<InteractionRelayComponent>(pilot);
			this._actions.RemoveProvidedActions(pilot, mech, null);
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x0001EFA4 File Offset: 0x0001D1A4
		[NullableContext(2)]
		public virtual void BreakMech(EntityUid uid, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return;
			}
			this.TryEject(uid, component);
			foreach (EntityUid ent in new List<EntityUid>(component.EquipmentContainer.ContainedEntities))
			{
				this.RemoveEquipment(uid, ent, component, null, true);
			}
			component.Broken = true;
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0001F02C File Offset: 0x0001D22C
		[NullableContext(2)]
		public void CycleEquipment(EntityUid uid, SharedMechComponent component = null)
		{
			SharedMechSystem.<>c__DisplayClass24_0 CS$<>8__locals1 = new SharedMechSystem.<>c__DisplayClass24_0();
			CS$<>8__locals1.component = component;
			if (!base.Resolve<SharedMechComponent>(uid, ref CS$<>8__locals1.component, true))
			{
				return;
			}
			List<EntityUid> allEquipment = CS$<>8__locals1.component.EquipmentContainer.ContainedEntities.ToList<EntityUid>();
			int equipmentIndex = -1;
			if (CS$<>8__locals1.component.CurrentSelectedEquipment != null)
			{
				equipmentIndex = allEquipment.FindIndex(new Predicate<EntityUid>(CS$<>8__locals1.<CycleEquipment>g__StartIndex|0));
			}
			equipmentIndex++;
			CS$<>8__locals1.component.CurrentSelectedEquipment = ((equipmentIndex >= allEquipment.Count) ? null : new EntityUid?(allEquipment[equipmentIndex]));
			string popupString = (CS$<>8__locals1.component.CurrentSelectedEquipment != null) ? Loc.GetString("mech-equipment-select-popup", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("item", CS$<>8__locals1.component.CurrentSelectedEquipment)
			}) : Loc.GetString("mech-equipment-select-none-popup");
			if (this._timing.IsFirstTimePredicted)
			{
				this._popup.PopupEntity(popupString, uid, PopupType.Small);
			}
			base.Dirty(CS$<>8__locals1.component, null);
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0001F13C File Offset: 0x0001D33C
		[NullableContext(2)]
		public void InsertEquipment(EntityUid uid, EntityUid toInsert, SharedMechComponent component = null, MechEquipmentComponent equipmentComponent = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return;
			}
			if (!base.Resolve<MechEquipmentComponent>(toInsert, ref equipmentComponent, true))
			{
				return;
			}
			if (component.EquipmentContainer.ContainedEntities.Count >= component.MaxEquipmentAmount)
			{
				return;
			}
			if (component.EquipmentWhitelist != null && !component.EquipmentWhitelist.IsValid(uid, null))
			{
				return;
			}
			equipmentComponent.EquipmentOwner = new EntityUid?(uid);
			component.EquipmentContainer.Insert(toInsert, this.EntityManager, null, null, null, null);
			MechEquipmentInsertedEvent ev = new MechEquipmentInsertedEvent(uid);
			base.RaiseLocalEvent<MechEquipmentInsertedEvent>(toInsert, ref ev, false);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0001F1D4 File Offset: 0x0001D3D4
		[NullableContext(2)]
		public void RemoveEquipment(EntityUid uid, EntityUid toRemove, SharedMechComponent component = null, MechEquipmentComponent equipmentComponent = null, bool forced = false)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return;
			}
			if (!base.Resolve<MechEquipmentComponent>(toRemove, ref equipmentComponent, true))
			{
				return;
			}
			if (!forced)
			{
				AttemptRemoveMechEquipmentEvent attemptev = new AttemptRemoveMechEquipmentEvent();
				base.RaiseLocalEvent<AttemptRemoveMechEquipmentEvent>(toRemove, ref attemptev, false);
				if (attemptev.Cancelled)
				{
					return;
				}
			}
			MechEquipmentRemovedEvent ev = new MechEquipmentRemovedEvent(uid);
			base.RaiseLocalEvent<MechEquipmentRemovedEvent>(toRemove, ref ev, false);
			EntityUid? currentSelectedEquipment = component.CurrentSelectedEquipment;
			if (currentSelectedEquipment != null && (currentSelectedEquipment == null || currentSelectedEquipment.GetValueOrDefault() == toRemove))
			{
				this.CycleEquipment(uid, component);
			}
			equipmentComponent.EquipmentOwner = null;
			component.EquipmentContainer.Remove(toRemove, this.EntityManager, null, null, true, false, null, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0001F2A4 File Offset: 0x0001D4A4
		[NullableContext(2)]
		public virtual bool TryChangeEnergy(EntityUid uid, FixedPoint2 delta, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.Energy + delta < 0)
			{
				return false;
			}
			component.Energy = FixedPoint2.Clamp(component.Energy + delta, 0, component.MaxEnergy);
			base.Dirty(component, null);
			this.UpdateUserInterface(uid, component);
			return true;
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0001F30C File Offset: 0x0001D50C
		[NullableContext(2)]
		public void SetIntegrity(EntityUid uid, FixedPoint2 value, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return;
			}
			component.Integrity = FixedPoint2.Clamp(value, 0, component.MaxIntegrity);
			if (component.Integrity <= 0)
			{
				this.BreakMech(uid, component);
			}
			else if (component.Broken)
			{
				component.Broken = false;
				this.UpdateAppearance(uid, component, null);
			}
			base.Dirty(component, null);
			this.UpdateUserInterface(uid, component);
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0001F380 File Offset: 0x0001D580
		public bool IsEmpty(SharedMechComponent component)
		{
			return component.PilotSlot.ContainedEntity == null;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0001F3A3 File Offset: 0x0001D5A3
		[NullableContext(2)]
		public bool CanInsert(EntityUid uid, EntityUid toInsert, SharedMechComponent component = null)
		{
			return base.Resolve<SharedMechComponent>(uid, ref component, true) && (this.IsEmpty(component) && this._actionBlocker.CanMove(toInsert, null)) && base.HasComp<SharedHandsComponent>(toInsert);
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0001F3D4 File Offset: 0x0001D5D4
		[NullableContext(2)]
		public virtual void UpdateUserInterface(EntityUid uid, SharedMechComponent component = null)
		{
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0001F3D8 File Offset: 0x0001D5D8
		[NullableContext(2)]
		public virtual bool TryInsert(EntityUid uid, EntityUid? toInsert, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (toInsert == null || component.PilotSlot.ContainedEntity == toInsert)
			{
				return false;
			}
			if (!this.CanInsert(uid, toInsert.Value, component))
			{
				return false;
			}
			this.SetupUser(uid, toInsert.Value, null);
			component.PilotSlot.Insert(toInsert.Value, this.EntityManager, null, null, null, null);
			this.UpdateAppearance(uid, component, null);
			return true;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0001F48C File Offset: 0x0001D68C
		[NullableContext(2)]
		public virtual bool TryEject(EntityUid uid, SharedMechComponent component = null)
		{
			if (!base.Resolve<SharedMechComponent>(uid, ref component, true))
			{
				return false;
			}
			if (component.PilotSlot.ContainedEntity == null)
			{
				return false;
			}
			EntityUid pilot = component.PilotSlot.ContainedEntity.Value;
			this.RemoveUser(uid, pilot);
			this._container.RemoveEntity(uid, pilot, null, null, null, true, false, null, null);
			this.UpdateAppearance(uid, component, null);
			return true;
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0001F508 File Offset: 0x0001D708
		private void OnGetMeleeWeapon(EntityUid uid, MechPilotComponent component, GetMeleeWeaponEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			SharedMechComponent mech;
			if (!base.TryComp<SharedMechComponent>(component.Mech, ref mech))
			{
				return;
			}
			EntityUid weapon = mech.CurrentSelectedEquipment ?? component.Mech;
			args.Weapon = new EntityUid?(weapon);
			args.Handled = true;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0001F562 File Offset: 0x0001D762
		private void OnCanAttackFromContainer(EntityUid uid, MechPilotComponent component, CanAttackFromContainerEvent args)
		{
			args.CanAttack = true;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0001F56C File Offset: 0x0001D76C
		private void OnAttackAttempt(EntityUid uid, MechPilotComponent component, AttackAttemptEvent args)
		{
			EntityUid? target = args.Target;
			EntityUid mech = component.Mech;
			if (target != null && (target == null || target.GetValueOrDefault() == mech))
			{
				args.Cancel();
			}
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0001F5B4 File Offset: 0x0001D7B4
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, SharedMechComponent component = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<SharedMechComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, MechVisuals.Open, this.IsEmpty(component), appearance);
			this._appearance.SetData(uid, MechVisuals.Broken, component.Broken, appearance);
		}

		// Token: 0x0400091B RID: 2331
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400091C RID: 2332
		[Dependency]
		private readonly IPrototypeManager _prototype;

		// Token: 0x0400091D RID: 2333
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x0400091E RID: 2334
		[Dependency]
		private readonly AccessReaderSystem _access;

		// Token: 0x0400091F RID: 2335
		[Dependency]
		private readonly SharedActionsSystem _actions;

		// Token: 0x04000920 RID: 2336
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000921 RID: 2337
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000922 RID: 2338
		[Dependency]
		private readonly SharedInteractionSystem _interaction;

		// Token: 0x04000923 RID: 2339
		[Dependency]
		private readonly SharedMoverController _mover;

		// Token: 0x04000924 RID: 2340
		[Dependency]
		private readonly SharedPopupSystem _popup;
	}
}
