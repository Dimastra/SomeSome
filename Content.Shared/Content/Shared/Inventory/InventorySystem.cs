using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Electrocution;
using Content.Shared.Explosion;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Radio;
using Content.Shared.Slippery;
using Content.Shared.Strip.Components;
using Content.Shared.Temperature;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Inventory
{
	// Token: 0x020003AC RID: 940
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class InventorySystem : EntitySystem
	{
		// Token: 0x06000AC3 RID: 2755 RVA: 0x00023148 File Offset: 0x00021348
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeEquip();
			this.InitializeRelay();
			this.InitializeSlots();
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x00023162 File Offset: 0x00021362
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownSlots();
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x00023170 File Offset: 0x00021370
		private void InitializeEquip()
		{
			base.SubscribeLocalEvent<InventoryComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<InventoryComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
			base.SubscribeLocalEvent<InventoryComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<InventoryComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved), null, null);
			base.SubscribeAllEvent<UseSlotNetworkMessage>(new EntitySessionEventHandler<UseSlotNetworkMessage>(this.OnUseSlot), null, null);
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x000231B0 File Offset: 0x000213B0
		protected void QuickEquip(EntityUid uid, ClothingComponent component, UseInHandEvent args)
		{
			InventoryComponent inv;
			SharedHandsComponent hands;
			InventoryTemplatePrototype prototype;
			if (!base.TryComp<InventoryComponent>(args.User, ref inv) || !base.TryComp<SharedHandsComponent>(args.User, ref hands) || !this._prototypeManager.TryIndex<InventoryTemplatePrototype>(inv.TemplateId, ref prototype))
			{
				return;
			}
			foreach (SlotDefinition slotDef in prototype.Slots)
			{
				string text;
				if (this.CanEquip(args.User, uid, slotDef.Name, out text, slotDef, inv, null, null))
				{
					EntityUid? slotEntity;
					if (this.TryGetSlotEntity(args.User, slotDef.Name, out slotEntity, inv, null))
					{
						ClothingComponent item;
						if ((base.TryComp<ClothingComponent>(slotEntity, ref item) && !item.QuickEquip) || !this.TryUnequip(args.User, slotDef.Name, true, false, false, inv, null) || !this.TryEquip(args.User, uid, slotDef.Name, true, false, false, inv, null))
						{
							goto IL_114;
						}
						this._handsSystem.PickupOrDrop(new EntityUid?(args.User), slotEntity.Value, true, false, null, null);
					}
					else if (!this.TryEquip(args.User, uid, slotDef.Name, true, false, false, inv, null))
					{
						goto IL_114;
					}
					args.Handled = true;
					return;
				}
				IL_114:;
			}
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x000232E4 File Offset: 0x000214E4
		private void OnEntRemoved(EntityUid uid, InventoryComponent component, EntRemovedFromContainerMessage args)
		{
			SlotDefinition slotDef;
			if (!this.TryGetSlot(uid, args.Container.ID, out slotDef, component))
			{
				return;
			}
			DidUnequipEvent unequippedEvent = new DidUnequipEvent(uid, args.Entity, slotDef);
			base.RaiseLocalEvent<DidUnequipEvent>(uid, unequippedEvent, true);
			GotUnequippedEvent gotUnequippedEvent = new GotUnequippedEvent(uid, args.Entity, slotDef);
			base.RaiseLocalEvent<GotUnequippedEvent>(args.Entity, gotUnequippedEvent, true);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0002333C File Offset: 0x0002153C
		private void OnEntInserted(EntityUid uid, InventoryComponent component, EntInsertedIntoContainerMessage args)
		{
			SlotDefinition slotDef;
			if (!this.TryGetSlot(uid, args.Container.ID, out slotDef, component))
			{
				return;
			}
			DidEquipEvent equippedEvent = new DidEquipEvent(uid, args.Entity, slotDef);
			base.RaiseLocalEvent<DidEquipEvent>(uid, equippedEvent, true);
			GotEquippedEvent gotEquippedEvent = new GotEquippedEvent(uid, args.Entity, slotDef);
			base.RaiseLocalEvent<GotEquippedEvent>(args.Entity, gotEquippedEvent, true);
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x00023394 File Offset: 0x00021594
		private void OnUseSlot(UseSlotNetworkMessage ev, EntitySessionEventArgs eventArgs)
		{
			EntityUid? attachedEntity = eventArgs.SenderSession.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid actor = attachedEntity.GetValueOrDefault();
				if (actor.Valid)
				{
					InventoryComponent inventory;
					SharedHandsComponent hands;
					if (!base.TryComp<InventoryComponent>(actor, ref inventory) || !base.TryComp<SharedHandsComponent>(actor, ref hands))
					{
						return;
					}
					EntityUid? held = hands.ActiveHandEntity;
					EntityUid? itemUid;
					this.TryGetSlotEntity(actor, ev.Slot, out itemUid, inventory, null);
					if (held != null && itemUid != null)
					{
						this._interactionSystem.InteractUsing(actor, held.Value, itemUid.Value, base.Transform(itemUid.Value).Coordinates, true, true);
						return;
					}
					if (itemUid != null)
					{
						EntityUid? item;
						if (!this.TryUnequip(actor, ev.Slot, out item, false, false, true, inventory, null))
						{
							return;
						}
						this._handsSystem.PickupOrDrop(new EntityUid?(actor), item.Value, true, false, null, null);
						return;
					}
					else
					{
						if (held == null)
						{
							return;
						}
						string reason;
						if (!this.CanEquip(actor, held.Value, ev.Slot, out reason, null, null, null, null))
						{
							if (this._gameTiming.IsFirstTimePredicted)
							{
								this._popup.PopupCursor(Loc.GetString(reason), PopupType.Small);
							}
							return;
						}
						if (!this._handsSystem.CanDropHeld(actor, hands.ActiveHand, false))
						{
							return;
						}
						GotUnequippedHandEvent gotUnequipped = new GotUnequippedHandEvent(actor, held.Value, hands.ActiveHand);
						DidUnequipHandEvent didUnequip = new DidUnequipHandEvent(actor, held.Value, hands.ActiveHand);
						base.RaiseLocalEvent<GotUnequippedHandEvent>(held.Value, gotUnequipped, false);
						base.RaiseLocalEvent<DidUnequipHandEvent>(actor, didUnequip, true);
						base.RaiseLocalEvent<HandDeselectedEvent>(held.Value, new HandDeselectedEvent(actor), false);
						this.TryEquip(actor, actor, held.Value, ev.Slot, false, true, true, inventory, null);
						return;
					}
				}
			}
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x00023548 File Offset: 0x00021748
		[NullableContext(2)]
		public bool TryEquip(EntityUid uid, EntityUid itemUid, [Nullable(1)] string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			return this.TryEquip(uid, uid, itemUid, slot, silent, force, predicted, inventory, clothing);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0002356C File Offset: 0x0002176C
		[NullableContext(2)]
		public bool TryEquip(EntityUid actor, EntityUid target, EntityUid itemUid, [Nullable(1)] string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			if (!base.Resolve<InventoryComponent>(target, ref inventory, false))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString("inventory-component-can-equip-cannot"), PopupType.Small);
				}
				return false;
			}
			base.Resolve<ClothingComponent>(itemUid, ref clothing, false);
			ContainerSlot slotContainer;
			SlotDefinition slotDefinition;
			if (!this.TryGetSlotContainer(target, slot, out slotContainer, out slotDefinition, inventory, null))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString("inventory-component-can-equip-cannot"), PopupType.Small);
				}
				return false;
			}
			string reason;
			if (!force && !this.CanEquip(actor, target, itemUid, slot, out reason, slotDefinition, inventory, clothing, null))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString(reason), PopupType.Small);
				}
				return false;
			}
			if (!slotContainer.Insert(itemUid, null, null, null, null, null))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString("inventory-component-can-unequip-cannot"), PopupType.Small);
				}
				return false;
			}
			if (!silent && clothing != null && clothing.EquipSound != null && this._gameTiming.IsFirstTimePredicted)
			{
				Filter filter;
				if (this._netMan.IsClient)
				{
					filter = Filter.Local();
				}
				else
				{
					filter = Filter.Pvs(target, 2f, null, null, null);
					if (predicted)
					{
						filter.RemoveWhereAttachedEntity((EntityUid entity) => entity == actor);
					}
				}
				SoundSystem.Play(clothing.EquipSound.GetSound(null, null), filter, target, new AudioParams?(clothing.EquipSound.Params.WithVolume(-2f)));
			}
			inventory.Dirty(null);
			this._movementSpeed.RefreshMovementSpeedModifiers(target, null);
			return true;
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0002372C File Offset: 0x0002192C
		public bool CanAccess(EntityUid actor, EntityUid target, EntityUid itemUid)
		{
			AttachedClothingComponent attachedComp;
			if (base.TryComp<AttachedClothingComponent>(itemUid, ref attachedComp))
			{
				itemUid = attachedComp.AttachedUid;
			}
			return (!(actor != target) || (this._interactionSystem.InRangeUnobstructed(actor, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._containerSystem.IsInSameOrParentContainer(actor, target))) && ((this._interactionSystem.InRangeUnobstructed(actor, itemUid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._containerSystem.IsInSameOrParentContainer(actor, itemUid)) || this._interactionSystem.CanAccessViaStorage(actor, itemUid) || (actor != target && base.HasComp<StrippableComponent>(target) && base.HasComp<StrippingComponent>(actor) && base.HasComp<SharedHandsComponent>(actor)));
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x000237E4 File Offset: 0x000219E4
		[NullableContext(2)]
		public bool CanEquip(EntityUid uid, EntityUid itemUid, [Nullable(1)] string slot, [NotNullWhen(false)] out string reason, SlotDefinition slotDefinition = null, InventoryComponent inventory = null, ClothingComponent clothing = null, ItemComponent item = null)
		{
			return this.CanEquip(uid, uid, itemUid, slot, out reason, slotDefinition, inventory, clothing, item);
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x00023808 File Offset: 0x00021A08
		[NullableContext(2)]
		public bool CanEquip(EntityUid actor, EntityUid target, EntityUid itemUid, [Nullable(1)] string slot, [NotNullWhen(false)] out string reason, SlotDefinition slotDefinition = null, InventoryComponent inventory = null, ClothingComponent clothing = null, ItemComponent item = null)
		{
			reason = "inventory-component-can-equip-cannot";
			if (!base.Resolve<InventoryComponent>(target, ref inventory, false))
			{
				return false;
			}
			base.Resolve<ClothingComponent, ItemComponent>(itemUid, ref clothing, ref item, false);
			if (slotDefinition == null && !this.TryGetSlot(target, slot, out slotDefinition, inventory))
			{
				return false;
			}
			EntityUid? entityUid;
			if (slotDefinition.DependsOn != null && !this.TryGetSlotEntity(target, slotDefinition.DependsOn, out entityUid, inventory, null))
			{
				return false;
			}
			bool fittingInPocket = slotDefinition.SlotFlags.HasFlag(SlotFlags.POCKET) && item != null && item.Size <= 12;
			if ((clothing == null && !fittingInPocket) || (clothing != null && !clothing.Slots.HasFlag(slotDefinition.SlotFlags) && !fittingInPocket))
			{
				reason = "inventory-component-can-equip-does-not-fit";
				return false;
			}
			if (!this.CanAccess(actor, target, itemUid))
			{
				reason = "interaction-system-user-interaction-cannot-reach";
				return false;
			}
			if (slotDefinition.Whitelist != null && !slotDefinition.Whitelist.IsValid(itemUid, null))
			{
				reason = "inventory-component-can-equip-does-not-fit";
				return false;
			}
			if (slotDefinition.Blacklist != null && slotDefinition.Blacklist.IsValid(itemUid, null))
			{
				reason = "inventory-component-can-equip-does-not-fit";
				return false;
			}
			IsEquippingAttemptEvent attemptEvent = new IsEquippingAttemptEvent(actor, target, itemUid, slotDefinition);
			base.RaiseLocalEvent<IsEquippingAttemptEvent>(target, attemptEvent, true);
			if (attemptEvent.Cancelled)
			{
				reason = (attemptEvent.Reason ?? reason);
				return false;
			}
			if (actor != target)
			{
				attemptEvent.Reason = null;
				base.RaiseLocalEvent<IsEquippingAttemptEvent>(actor, attemptEvent, true);
				if (attemptEvent.Cancelled)
				{
					reason = (attemptEvent.Reason ?? reason);
					return false;
				}
			}
			BeingEquippedAttemptEvent itemAttemptEvent = new BeingEquippedAttemptEvent(actor, target, itemUid, slotDefinition);
			base.RaiseLocalEvent<BeingEquippedAttemptEvent>(itemUid, itemAttemptEvent, true);
			if (itemAttemptEvent.Cancelled)
			{
				reason = (itemAttemptEvent.Reason ?? reason);
				return false;
			}
			return true;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x000239C8 File Offset: 0x00021BC8
		[NullableContext(2)]
		public bool TryUnequip(EntityUid uid, [Nullable(1)] string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			return this.TryUnequip(uid, uid, slot, silent, force, predicted, inventory, clothing);
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x000239E8 File Offset: 0x00021BE8
		[NullableContext(2)]
		public bool TryUnequip(EntityUid actor, EntityUid target, [Nullable(1)] string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			EntityUid? entityUid;
			return this.TryUnequip(actor, target, slot, out entityUid, silent, force, predicted, inventory, clothing);
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x00023A0C File Offset: 0x00021C0C
		[NullableContext(2)]
		public bool TryUnequip(EntityUid uid, [Nullable(1)] string slot, [NotNullWhen(true)] out EntityUid? removedItem, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			return this.TryUnequip(uid, uid, slot, out removedItem, silent, force, predicted, inventory, clothing);
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x00023A30 File Offset: 0x00021C30
		[NullableContext(2)]
		public bool TryUnequip(EntityUid actor, EntityUid target, [Nullable(1)] string slot, [NotNullWhen(true)] out EntityUid? removedItem, bool silent = false, bool force = false, bool predicted = false, InventoryComponent inventory = null, ClothingComponent clothing = null)
		{
			removedItem = null;
			if (!base.Resolve<InventoryComponent>(target, ref inventory, false))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString("inventory-component-can-unequip-cannot"), PopupType.Small);
				}
				return false;
			}
			ContainerSlot slotContainer;
			SlotDefinition slotDefinition;
			if (!this.TryGetSlotContainer(target, slot, out slotContainer, out slotDefinition, inventory, null))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString("inventory-component-can-unequip-cannot"), PopupType.Small);
				}
				return false;
			}
			removedItem = slotContainer.ContainedEntity;
			if (removedItem == null)
			{
				return false;
			}
			string reason;
			if (!force && !this.CanUnequip(actor, target, slot, out reason, slotContainer, slotDefinition, inventory))
			{
				if (!silent && this._gameTiming.IsFirstTimePredicted)
				{
					this._popup.PopupCursor(Loc.GetString(reason), PopupType.Small);
				}
				return false;
			}
			if (!force && !slotContainer.CanRemove(removedItem.Value, null))
			{
				return false;
			}
			foreach (SlotDefinition slotDef in this.GetSlots(target, inventory))
			{
				if (slotDef != slotDefinition && slotDef.DependsOn == slotDefinition.Name)
				{
					this.TryUnequip(actor, target, slotDef.Name, true, true, predicted, inventory, null);
				}
			}
			if (force)
			{
				slotContainer.ForceRemove(removedItem.Value, null, null);
			}
			else if (!slotContainer.Remove(removedItem.Value, null, null, null, true, false, null, null))
			{
				return false;
			}
			base.Transform(removedItem.Value).Coordinates = base.Transform(target).Coordinates;
			if (!silent && base.Resolve<ClothingComponent>(removedItem.Value, ref clothing, false) && clothing.UnequipSound != null && this._gameTiming.IsFirstTimePredicted)
			{
				Filter filter;
				if (this._netMan.IsClient)
				{
					filter = Filter.Local();
				}
				else
				{
					filter = Filter.Pvs(target, 2f, null, null, null);
					if (predicted)
					{
						filter.RemoveWhereAttachedEntity((EntityUid entity) => entity == actor);
					}
				}
				SoundSystem.Play(clothing.UnequipSound.GetSound(null, null), filter, target, new AudioParams?(clothing.UnequipSound.Params.WithVolume(-2f)));
			}
			inventory.Dirty(null);
			this._movementSpeed.RefreshMovementSpeedModifiers(target, null);
			return true;
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00023CA4 File Offset: 0x00021EA4
		[NullableContext(2)]
		public bool CanUnequip(EntityUid uid, [Nullable(1)] string slot, [NotNullWhen(false)] out string reason, ContainerSlot containerSlot = null, SlotDefinition slotDefinition = null, InventoryComponent inventory = null)
		{
			return this.CanUnequip(uid, uid, slot, out reason, containerSlot, slotDefinition, inventory);
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x00023CB8 File Offset: 0x00021EB8
		[NullableContext(2)]
		public bool CanUnequip(EntityUid actor, EntityUid target, [Nullable(1)] string slot, [NotNullWhen(false)] out string reason, ContainerSlot containerSlot = null, SlotDefinition slotDefinition = null, InventoryComponent inventory = null)
		{
			reason = "inventory-component-can-unequip-cannot";
			if (!base.Resolve<InventoryComponent>(target, ref inventory, false))
			{
				return false;
			}
			if ((containerSlot == null || slotDefinition == null) && !this.TryGetSlotContainer(target, slot, out containerSlot, out slotDefinition, inventory, null))
			{
				return false;
			}
			if (containerSlot.ContainedEntity == null)
			{
				return false;
			}
			if (containerSlot.ContainedEntity == null || !containerSlot.CanRemove(containerSlot.ContainedEntity.Value, null))
			{
				return false;
			}
			EntityUid itemUid = containerSlot.ContainedEntity.Value;
			if (!this.CanAccess(actor, target, itemUid))
			{
				reason = "interaction-system-user-interaction-cannot-reach";
				return false;
			}
			IsUnequippingAttemptEvent attemptEvent = new IsUnequippingAttemptEvent(actor, target, itemUid, slotDefinition);
			base.RaiseLocalEvent<IsUnequippingAttemptEvent>(target, attemptEvent, true);
			if (attemptEvent.Cancelled)
			{
				reason = (attemptEvent.Reason ?? reason);
				return false;
			}
			if (actor != target)
			{
				attemptEvent.Reason = null;
				base.RaiseLocalEvent<IsUnequippingAttemptEvent>(actor, attemptEvent, true);
				if (attemptEvent.Cancelled)
				{
					reason = (attemptEvent.Reason ?? reason);
					return false;
				}
			}
			BeingUnequippedAttemptEvent itemAttemptEvent = new BeingUnequippedAttemptEvent(actor, target, itemUid, slotDefinition);
			base.RaiseLocalEvent<BeingUnequippedAttemptEvent>(itemUid, itemAttemptEvent, true);
			if (itemAttemptEvent.Cancelled)
			{
				reason = (attemptEvent.Reason ?? reason);
				return false;
			}
			return true;
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x00023DEC File Offset: 0x00021FEC
		[NullableContext(2)]
		public bool TryGetSlotEntity(EntityUid uid, [Nullable(1)] string slot, [NotNullWhen(true)] out EntityUid? entityUid, InventoryComponent inventoryComponent = null, ContainerManagerComponent containerManagerComponent = null)
		{
			entityUid = null;
			ContainerSlot container;
			SlotDefinition slotDefinition;
			if (!base.Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventoryComponent, ref containerManagerComponent, false) || !this.TryGetSlotContainer(uid, slot, out container, out slotDefinition, inventoryComponent, containerManagerComponent))
			{
				return false;
			}
			entityUid = container.ContainedEntity;
			return entityUid != null;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x00023E34 File Offset: 0x00022034
		public bool SpawnItemInSlot(EntityUid uid, string slot, string prototype, bool silent = false, bool force = false, [Nullable(2)] InventoryComponent inventory = null)
		{
			InventorySystem.<>c__DisplayClass26_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			if (!base.Resolve<InventoryComponent>(uid, ref inventory, false))
			{
				return false;
			}
			if (base.Deleted(uid, null))
			{
				return false;
			}
			EntityUid? entityUid;
			if (!this.HasSlot(uid, slot, null) || this.TryGetSlotEntity(uid, slot, out entityUid, inventory, null))
			{
				return false;
			}
			if (!this._prototypeManager.HasIndex<EntityPrototype>(prototype))
			{
				return false;
			}
			CS$<>8__locals1.item = this.EntityManager.SpawnEntity(prototype, base.Transform(uid).Coordinates);
			return this.TryEquip(uid, CS$<>8__locals1.item, slot, silent, force, false, null, null) || this.<SpawnItemInSlot>g__DeleteItem|26_0(ref CS$<>8__locals1);
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x00023ED0 File Offset: 0x000220D0
		public void InitializeRelay()
		{
			base.SubscribeLocalEvent<InventoryComponent, DamageModifyEvent>(new ComponentEventHandler<InventoryComponent, DamageModifyEvent>(this.RelayInventoryEvent<DamageModifyEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, ElectrocutionAttemptEvent>(new ComponentEventHandler<InventoryComponent, ElectrocutionAttemptEvent>(this.RelayInventoryEvent<ElectrocutionAttemptEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, SlipAttemptEvent>(new ComponentEventHandler<InventoryComponent, SlipAttemptEvent>(this.RelayInventoryEvent<SlipAttemptEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<InventoryComponent, RefreshMovementSpeedModifiersEvent>(this.RelayInventoryEvent<RefreshMovementSpeedModifiersEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, GetExplosionResistanceEvent>(new ComponentEventHandler<InventoryComponent, GetExplosionResistanceEvent>(this.RelayInventoryEvent<GetExplosionResistanceEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, BeforeStripEvent>(new ComponentEventHandler<InventoryComponent, BeforeStripEvent>(this.RelayInventoryEvent<BeforeStripEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, SeeIdentityAttemptEvent>(new ComponentEventHandler<InventoryComponent, SeeIdentityAttemptEvent>(this.RelayInventoryEvent<SeeIdentityAttemptEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, ModifyChangedTemperatureEvent>(new ComponentEventHandler<InventoryComponent, ModifyChangedTemperatureEvent>(this.RelayInventoryEvent<ModifyChangedTemperatureEvent>), null, null);
			base.SubscribeLocalEvent<InventoryComponent, GetDefaultRadioChannelEvent>(new ComponentEventHandler<InventoryComponent, GetDefaultRadioChannelEvent>(this.RelayInventoryEvent<GetDefaultRadioChannelEvent>), null, null);
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x00023F94 File Offset: 0x00022194
		protected void RelayInventoryEvent<[Nullable(0)] T>(EntityUid uid, InventoryComponent component, T args) where T : EntityEventArgs, IInventoryRelayEvent
		{
			if (args.TargetSlots == SlotFlags.NONE)
			{
				return;
			}
			InventorySystem.ContainerSlotEnumerator containerEnumerator = new InventorySystem.ContainerSlotEnumerator(uid, component.TemplateId, this._prototypeManager, this, args.TargetSlots);
			InventoryRelayedEvent<T> ev = new InventoryRelayedEvent<T>(args);
			ContainerSlot container;
			while (containerEnumerator.MoveNext(out container))
			{
				if (container.ContainedEntity != null)
				{
					base.RaiseLocalEvent<InventoryRelayedEvent<T>>(container.ContainedEntity.Value, ev, false);
				}
			}
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x00024009 File Offset: 0x00022209
		private void InitializeSlots()
		{
			base.SubscribeLocalEvent<InventoryComponent, ComponentInit>(new ComponentEventHandler<InventoryComponent, ComponentInit>(this.OnInit), null, null);
			this._vvm.GetTypeHandler<InventoryComponent>().AddHandler(new HandleTypePathComponent<InventoryComponent>(this.HandleViewVariablesSlots), new ListTypeCustomPathsComponent<InventoryComponent>(this.ListViewVariablesSlots));
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x00024049 File Offset: 0x00022249
		private void ShutdownSlots()
		{
			this._vvm.GetTypeHandler<InventoryComponent>().RemoveHandler(new HandleTypePathComponent<InventoryComponent>(this.HandleViewVariablesSlots), new ListTypeCustomPathsComponent<InventoryComponent>(this.ListViewVariablesSlots));
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x00024074 File Offset: 0x00022274
		protected virtual void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
		{
			InventoryTemplatePrototype invTemplate;
			if (!this._prototypeManager.TryIndex<InventoryTemplatePrototype>(component.TemplateId, ref invTemplate))
			{
				return;
			}
			foreach (SlotDefinition slot in invTemplate.Slots)
			{
				this._containerSystem.EnsureContainer<ContainerSlot>(uid, slot.Name, null).OccludesLight = false;
			}
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x000240CC File Offset: 0x000222CC
		[NullableContext(2)]
		public bool TryGetSlotContainer(EntityUid uid, [Nullable(1)] string slot, [NotNullWhen(true)] out ContainerSlot containerSlot, [NotNullWhen(true)] out SlotDefinition slotDefinition, InventoryComponent inventory = null, ContainerManagerComponent containerComp = null)
		{
			containerSlot = null;
			slotDefinition = null;
			if (!base.Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventory, ref containerComp, false))
			{
				return false;
			}
			if (!this.TryGetSlot(uid, slot, out slotDefinition, inventory))
			{
				return false;
			}
			IContainer container;
			if (!containerComp.TryGetContainer(slotDefinition.Name, ref container))
			{
				if (inventory.LifeStage >= 4)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Missing inventory container ");
					defaultInterpolatedStringHandler.AppendFormatted(slot);
					defaultInterpolatedStringHandler.AppendLiteral(" on entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				return false;
			}
			ContainerSlot containerSlotChecked = container as ContainerSlot;
			if (containerSlotChecked == null)
			{
				return false;
			}
			containerSlot = containerSlotChecked;
			return true;
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x00024174 File Offset: 0x00022374
		public bool HasSlot(EntityUid uid, string slot, [Nullable(2)] InventoryComponent component = null)
		{
			SlotDefinition slotDefinition;
			return this.TryGetSlot(uid, slot, out slotDefinition, component);
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x0002418C File Offset: 0x0002238C
		[NullableContext(2)]
		public bool TryGetSlot(EntityUid uid, [Nullable(1)] string slot, [NotNullWhen(true)] out SlotDefinition slotDefinition, InventoryComponent inventory = null)
		{
			slotDefinition = null;
			if (!base.Resolve<InventoryComponent>(uid, ref inventory, false))
			{
				return false;
			}
			InventoryTemplatePrototype templatePrototype;
			if (!this._prototypeManager.TryIndex<InventoryTemplatePrototype>(inventory.TemplateId, ref templatePrototype))
			{
				return false;
			}
			foreach (SlotDefinition slotDef in templatePrototype.Slots)
			{
				if (slotDef.Name.Equals(slot))
				{
					slotDefinition = slotDef;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x000241EF File Offset: 0x000223EF
		[NullableContext(2)]
		public bool TryGetContainerSlotEnumerator(EntityUid uid, out InventorySystem.ContainerSlotEnumerator containerSlotEnumerator, InventoryComponent component = null)
		{
			containerSlotEnumerator = default(InventorySystem.ContainerSlotEnumerator);
			if (!base.Resolve<InventoryComponent>(uid, ref component, false))
			{
				return false;
			}
			containerSlotEnumerator = new InventorySystem.ContainerSlotEnumerator(uid, component.TemplateId, this._prototypeManager, this, SlotFlags.All);
			return true;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00024224 File Offset: 0x00022424
		[NullableContext(2)]
		public bool TryGetSlots(EntityUid uid, [Nullable(new byte[]
		{
			2,
			1
		})] [NotNullWhen(true)] out SlotDefinition[] slotDefinitions, InventoryComponent inventoryComponent = null)
		{
			slotDefinitions = null;
			if (!base.Resolve<InventoryComponent>(uid, ref inventoryComponent, false))
			{
				return false;
			}
			InventoryTemplatePrototype templatePrototype;
			if (!this._prototypeManager.TryIndex<InventoryTemplatePrototype>(inventoryComponent.TemplateId, ref templatePrototype))
			{
				return false;
			}
			slotDefinitions = templatePrototype.Slots;
			return true;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00024262 File Offset: 0x00022462
		public SlotDefinition[] GetSlots(EntityUid uid, [Nullable(2)] InventoryComponent inventoryComponent = null)
		{
			if (!base.Resolve<InventoryComponent>(uid, ref inventoryComponent, true))
			{
				throw new InvalidOperationException();
			}
			return this._prototypeManager.Index<InventoryTemplatePrototype>(inventoryComponent.TemplateId).Slots;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0002428C File Offset: 0x0002248C
		[return: Nullable(2)]
		private ViewVariablesPath HandleViewVariablesSlots(EntityUid uid, InventoryComponent comp, string relativePath)
		{
			EntityUid? entity;
			if (!this.TryGetSlotEntity(uid, relativePath, out entity, comp, null))
			{
				return null;
			}
			return ViewVariablesPath.FromObject(entity);
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x000242B4 File Offset: 0x000224B4
		private IEnumerable<string> ListViewVariablesSlots(EntityUid uid, InventoryComponent comp)
		{
			foreach (SlotDefinition slotDef in this.GetSlots(uid, comp))
			{
				yield return slotDef.Name;
			}
			SlotDefinition[] array = null;
			yield break;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x000242DA File Offset: 0x000224DA
		[CompilerGenerated]
		private bool <SpawnItemInSlot>g__DeleteItem|26_0(ref InventorySystem.<>c__DisplayClass26_0 A_1)
		{
			this.EntityManager.DeleteEntity(A_1.item);
			return false;
		}

		// Token: 0x04000AB4 RID: 2740
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000AB5 RID: 2741
		[Dependency]
		private readonly MovementSpeedModifierSystem _movementSpeed;

		// Token: 0x04000AB6 RID: 2742
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000AB7 RID: 2743
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x04000AB8 RID: 2744
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04000AB9 RID: 2745
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000ABA RID: 2746
		[Dependency]
		private readonly INetManager _netMan;

		// Token: 0x04000ABB RID: 2747
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000ABC RID: 2748
		[Dependency]
		private readonly IViewVariablesManager _vvm;

		// Token: 0x020007E9 RID: 2025
		[Nullable(0)]
		public struct ContainerSlotEnumerator
		{
			// Token: 0x0600186A RID: 6250 RVA: 0x0004E0B4 File Offset: 0x0004C2B4
			public ContainerSlotEnumerator(EntityUid uid, string prototypeId, IPrototypeManager prototypeManager, InventorySystem inventorySystem, SlotFlags flags = SlotFlags.All)
			{
				this._nextIdx = 0;
				this._uid = uid;
				this._inventorySystem = inventorySystem;
				this._flags = flags;
				InventoryTemplatePrototype prototype;
				if (prototypeManager.TryIndex<InventoryTemplatePrototype>(prototypeId, ref prototype))
				{
					this._slots = prototype.Slots;
					return;
				}
				this._slots = Array.Empty<SlotDefinition>();
			}

			// Token: 0x0600186B RID: 6251 RVA: 0x0004E104 File Offset: 0x0004C304
			[NullableContext(2)]
			public bool MoveNext([NotNullWhen(true)] out ContainerSlot container)
			{
				container = null;
				while (this._nextIdx < this._slots.Length)
				{
					SlotDefinition slot = this._slots[this._nextIdx];
					this._nextIdx++;
					SlotDefinition slotDefinition;
					if ((slot.SlotFlags & this._flags) != SlotFlags.NONE && this._inventorySystem.TryGetSlotContainer(this._uid, slot.Name, out container, out slotDefinition, null, null))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x04001855 RID: 6229
			private readonly InventorySystem _inventorySystem;

			// Token: 0x04001856 RID: 6230
			private readonly EntityUid _uid;

			// Token: 0x04001857 RID: 6231
			private readonly SlotDefinition[] _slots;

			// Token: 0x04001858 RID: 6232
			private readonly SlotFlags _flags;

			// Token: 0x04001859 RID: 6233
			private int _nextIdx;
		}
	}
}
