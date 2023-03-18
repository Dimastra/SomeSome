using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Clothing.Components;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing.EntitySystems
{
	// Token: 0x020005AD RID: 1453
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToggleableClothingSystem : EntitySystem
	{
		// Token: 0x060011C3 RID: 4547 RVA: 0x0003A2B8 File Offset: 0x000384B8
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ToggleableClothingComponent, ComponentInit>(new ComponentEventHandler<ToggleableClothingComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<ToggleableClothingComponent, MapInitEvent>(new ComponentEventHandler<ToggleableClothingComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<ToggleableClothingComponent, ToggleClothingEvent>(new ComponentEventHandler<ToggleableClothingComponent, ToggleClothingEvent>(this.OnToggleClothing), null, null);
			base.SubscribeLocalEvent<ToggleableClothingComponent, GetItemActionsEvent>(new ComponentEventHandler<ToggleableClothingComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<ToggleableClothingComponent, ComponentRemove>(new ComponentEventHandler<ToggleableClothingComponent, ComponentRemove>(this.OnRemoveToggleable), null, null);
			base.SubscribeLocalEvent<ToggleableClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ToggleableClothingComponent, GotUnequippedEvent>(this.OnToggleableUnequip), null, null);
			base.SubscribeLocalEvent<AttachedClothingComponent, InteractHandEvent>(new ComponentEventHandler<AttachedClothingComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<AttachedClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<AttachedClothingComponent, GotUnequippedEvent>(this.OnAttachedUnequip), null, null);
			base.SubscribeLocalEvent<AttachedClothingComponent, ComponentRemove>(new ComponentEventHandler<AttachedClothingComponent, ComponentRemove>(this.OnRemoveAttached), null, null);
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x0003A380 File Offset: 0x00038580
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			EntityUid uid;
			while (this._toInsert.TryDequeue(out uid))
			{
				ToggleableClothingComponent component;
				if (base.TryComp<ToggleableClothingComponent>(uid, ref component) && component.ClothingUid != null)
				{
					ContainerSlot container = component.Container;
					if (container != null)
					{
						container.Insert(component.ClothingUid.Value, null, null, null, null, null);
					}
				}
			}
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x0003A3E0 File Offset: 0x000385E0
		private void OnInteractHand(EntityUid uid, AttachedClothingComponent component, InteractHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToggleableClothingComponent toggleCom;
			if (!base.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleCom) || toggleCom.Container == null)
			{
				return;
			}
			if (!this._inventorySystem.TryUnequip(base.Transform(uid).ParentUid, toggleCom.Slot, false, true, false, null, null))
			{
				return;
			}
			toggleCom.Container.Insert(uid, this.EntityManager, null, null, null, null);
			args.Handled = true;
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x0003A454 File Offset: 0x00038654
		private void OnToggleableUnequip(EntityUid uid, ToggleableClothingComponent component, GotUnequippedEvent args)
		{
			if (component.Container != null && component.Container.ContainedEntity == null && component.ClothingUid != null)
			{
				this._inventorySystem.TryUnequip(args.Equipee, component.Slot, false, true, false, null, null);
			}
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x0003A4A8 File Offset: 0x000386A8
		private void OnRemoveToggleable(EntityUid uid, ToggleableClothingComponent component, ComponentRemove args)
		{
			InstantAction toggleAction = component.ToggleAction;
			if (toggleAction != null && toggleAction.AttachedEntity != null)
			{
				this._actionsSystem.RemoveAction(component.ToggleAction.AttachedEntity.Value, component.ToggleAction, null);
			}
			if (component.ClothingUid != null)
			{
				base.QueueDel(component.ClothingUid.Value);
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0003A510 File Offset: 0x00038710
		private void OnRemoveAttached(EntityUid uid, AttachedClothingComponent component, ComponentRemove args)
		{
			ToggleableClothingComponent toggleComp;
			if (!base.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleComp))
			{
				return;
			}
			if (toggleComp.LifeStage > 6)
			{
				return;
			}
			InstantAction toggleAction = toggleComp.ToggleAction;
			if (toggleAction != null && toggleAction.AttachedEntity != null)
			{
				this._actionsSystem.RemoveAction(toggleComp.ToggleAction.AttachedEntity.Value, toggleComp.ToggleAction, null);
			}
			base.RemComp(component.AttachedUid, toggleComp);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x0003A580 File Offset: 0x00038780
		private void OnAttachedUnequip(EntityUid uid, AttachedClothingComponent component, GotUnequippedEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			ToggleableClothingComponent toggleComp;
			if (!base.TryComp<ToggleableClothingComponent>(component.AttachedUid, ref toggleComp))
			{
				return;
			}
			if (toggleComp.LifeStage > 6)
			{
				return;
			}
			this._toInsert.Enqueue(component.AttachedUid);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x0003A5C4 File Offset: 0x000387C4
		private void OnToggleClothing(EntityUid uid, ToggleableClothingComponent component, ToggleClothingEvent args)
		{
			if (args.Handled || component.Container == null || component.ClothingUid == null)
			{
				return;
			}
			EntityUid parent = base.Transform(uid).ParentUid;
			EntityUid? existing;
			if (component.Container.ContainedEntity == null)
			{
				this._inventorySystem.TryUnequip(parent, component.Slot, false, false, false, null, null);
			}
			else if (this._inventorySystem.TryGetSlotEntity(parent, component.Slot, out existing, null, null))
			{
				this._popupSystem.PopupEntity(Loc.GetString("toggleable-clothing-remove-first", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("entity", existing)
				}), args.Performer, args.Performer, PopupType.Small);
			}
			else
			{
				this._inventorySystem.TryEquip(parent, component.ClothingUid.Value, component.Slot, false, false, false, null, null);
			}
			args.Handled = true;
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0003A6B0 File Offset: 0x000388B0
		private void OnGetActions(EntityUid uid, ToggleableClothingComponent component, GetItemActionsEvent args)
		{
			if (component.ClothingUid != null)
			{
				SlotFlags? slotFlags = args.SlotFlags & component.RequiredFlags;
				SlotFlags requiredFlags = component.RequiredFlags;
				if (slotFlags.GetValueOrDefault() == requiredFlags & slotFlags != null)
				{
					if (component.ToggleAction != null)
					{
						args.Actions.Add(component.ToggleAction);
					}
					return;
				}
			}
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0003A732 File Offset: 0x00038932
		private void OnInit(EntityUid uid, ToggleableClothingComponent component, ComponentInit args)
		{
			component.Container = this._containerSystem.EnsureContainer<ContainerSlot>(uid, component.ContainerId, null);
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0003A750 File Offset: 0x00038950
		private void OnMapInit(EntityUid uid, ToggleableClothingComponent component, MapInitEvent args)
		{
			EntityUid? containedEntity = component.Container.ContainedEntity;
			if (containedEntity != null)
			{
				containedEntity.GetValueOrDefault();
				return;
			}
			InstantActionPrototype act;
			if (component.ToggleAction == null && this._proto.TryIndex<InstantActionPrototype>(component.ActionId, ref act))
			{
				component.ToggleAction = new InstantAction(act);
			}
			if (component.ClothingUid == null || component.ToggleAction == null)
			{
				TransformComponent xform = base.Transform(uid);
				component.ClothingUid = new EntityUid?(base.Spawn(component.ClothingPrototype, xform.Coordinates));
				base.EnsureComp<AttachedClothingComponent>(component.ClothingUid.Value).AttachedUid = uid;
				component.Container.Insert(component.ClothingUid.Value, this.EntityManager, null, xform, null, null);
			}
			if (component.ToggleAction != null)
			{
				component.ToggleAction.EntityIcon = component.ClothingUid;
				this._actionsSystem.Dirty(component.ToggleAction);
			}
		}

		// Token: 0x0400105E RID: 4190
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;

		// Token: 0x0400105F RID: 4191
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x04001060 RID: 4192
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04001061 RID: 4193
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x04001062 RID: 4194
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x04001063 RID: 4195
		private Queue<EntityUid> _toInsert = new Queue<EntityUid>();
	}
}
