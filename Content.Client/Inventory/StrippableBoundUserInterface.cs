using System;
using System.Runtime.CompilerServices;
using Content.Client.Cuffs.Components;
using Content.Client.Examine;
using Content.Client.Hands;
using Content.Client.Strip;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Strip.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Inventory
{
	// Token: 0x020002A7 RID: 679
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StrippableBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001131 RID: 4401 RVA: 0x0006617C File Offset: 0x0006437C
		public StrippableBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<StrippableBoundUserInterface>(this);
			this._examine = this._entMan.EntitySysManager.GetEntitySystem<ExamineSystem>();
			this._inv = this._entMan.EntitySysManager.GetEntitySystem<InventorySystem>();
			string @string = Loc.GetString("strippable-bound-user-interface-stripping-menu-title", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("ownerName", Identity.Name(base.Owner.Owner, this._entMan, null))
			});
			this._strippingMenu = new StrippingMenu(@string, this);
			this._strippingMenu.OnClose += base.Close;
			this._virtualHiddenEntity = this._entMan.SpawnEntity("StrippingHiddenEntity", MapCoordinates.Nullspace);
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00066244 File Offset: 0x00064444
		protected override void Open()
		{
			base.Open();
			StrippingMenu strippingMenu = this._strippingMenu;
			if (strippingMenu == null)
			{
				return;
			}
			strippingMenu.OpenCenteredLeft();
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x0006625C File Offset: 0x0006445C
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this._entMan.DeleteEntity(this._virtualHiddenEntity);
			if (!disposing)
			{
				return;
			}
			StrippingMenu strippingMenu = this._strippingMenu;
			if (strippingMenu == null)
			{
				return;
			}
			strippingMenu.Dispose();
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0006628A File Offset: 0x0006448A
		public void DirtyMenu()
		{
			if (this._strippingMenu != null)
			{
				this._strippingMenu.Dirty = true;
			}
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x000662A0 File Offset: 0x000644A0
		public void UpdateMenu()
		{
			if (this._strippingMenu == null)
			{
				return;
			}
			this._strippingMenu.ClearButtons();
			InventoryComponent inventoryComponent;
			InventoryTemplatePrototype inventoryTemplatePrototype;
			if (this._entMan.TryGetComponent<InventoryComponent>(base.Owner.Owner, ref inventoryComponent) && this._protoMan.TryIndex<InventoryTemplatePrototype>(inventoryComponent.TemplateId, ref inventoryTemplatePrototype))
			{
				foreach (SlotDefinition slotDefinition in inventoryTemplatePrototype.Slots)
				{
					this.AddInventoryButton(slotDefinition.Name, inventoryTemplatePrototype, inventoryComponent);
				}
			}
			HandsComponent handsComponent;
			if (this._entMan.TryGetComponent<HandsComponent>(base.Owner.Owner, ref handsComponent))
			{
				foreach (Hand hand in handsComponent.Hands.Values)
				{
					if (hand.Location == HandLocation.Right)
					{
						this.AddHandButton(hand);
					}
				}
				foreach (Hand hand2 in handsComponent.Hands.Values)
				{
					if (hand2.Location == HandLocation.Middle)
					{
						this.AddHandButton(hand2);
					}
				}
				foreach (Hand hand3 in handsComponent.Hands.Values)
				{
					if (hand3.Location == HandLocation.Left)
					{
						this.AddHandButton(hand3);
					}
				}
			}
			EnsnareableComponent ensnareableComponent;
			if (this._entMan.TryGetComponent<EnsnareableComponent>(base.Owner.Owner, ref ensnareableComponent) && ensnareableComponent.IsEnsnared)
			{
				Button button = new Button
				{
					Text = Loc.GetString("strippable-bound-user-interface-stripping-menu-ensnare-button"),
					StyleClasses = 
					{
						"OpenRight"
					}
				};
				button.OnPressed += delegate(BaseButton.ButtonEventArgs _)
				{
					base.SendMessage(new StrippingEnsnareButtonPressed());
				};
				this._strippingMenu.SnareContainer.AddChild(button);
			}
			this._strippingMenu.SetSize = new ValueTuple<float, float>(220f, (float)((ensnareableComponent != null && ensnareableComponent.IsEnsnared) ? 550 : 530));
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x000664E0 File Offset: 0x000646E0
		private void AddHandButton(Hand hand)
		{
			HandButton handButton = new HandButton(hand.Name, hand.Location);
			handButton.Pressed += this.SlotPressed;
			HandVirtualItemComponent handVirtualItemComponent;
			if (this._entMan.TryGetComponent<HandVirtualItemComponent>(hand.HeldEntity, ref handVirtualItemComponent))
			{
				handButton.Blocked = true;
				CuffableComponent cuffableComponent;
				if (this._entMan.TryGetComponent<CuffableComponent>(base.Owner.Owner, ref cuffableComponent) && cuffableComponent.Container.Contains(handVirtualItemComponent.BlockingEntity))
				{
					handButton.BlockedRect.MouseFilter = 2;
				}
			}
			this.UpdateEntityIcon(handButton, hand.HeldEntity);
			this._strippingMenu.HandsContainer.AddChild(handButton);
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00066584 File Offset: 0x00064784
		private void SlotPressed(GUIBoundKeyEventArgs ev, SlotControl slot)
		{
			if (ev.Function == EngineKeyFunctions.Use)
			{
				base.SendMessage(new StrippingSlotButtonPressed(slot.SlotName, slot is HandButton));
			}
			else if (ev.Function == ContentKeyFunctions.ExamineEntity && slot.Entity != null)
			{
				this._examine.DoExamine(slot.Entity.Value, true);
				return;
			}
			ev.Function != EngineKeyFunctions.Use;
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00066610 File Offset: 0x00064810
		private void AddInventoryButton(string slotId, InventoryTemplatePrototype template, InventoryComponent inv)
		{
			ContainerSlot containerSlot;
			SlotDefinition slotDefinition;
			if (!this._inv.TryGetSlotContainer(inv.Owner, slotId, out containerSlot, out slotDefinition, inv, null))
			{
				return;
			}
			EntityUid? containedEntity = containerSlot.ContainedEntity;
			if (containedEntity != null && slotDefinition.StripHidden)
			{
				containedEntity = new EntityUid?(this._virtualHiddenEntity);
			}
			SlotButton slotButton = new SlotButton(new ClientInventorySystem.SlotData(slotDefinition, containerSlot, false, false));
			slotButton.Pressed += this.SlotPressed;
			this._strippingMenu.InventoryContainer.AddChild(slotButton);
			this.UpdateEntityIcon(slotButton, containedEntity);
			LayoutContainer.SetPosition(slotButton, slotDefinition.StrippingWindowPos * (SlotControl.DefaultButtonSize + 4));
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x000666B4 File Offset: 0x000648B4
		private void UpdateEntityIcon(SlotControl button, EntityUid? entity)
		{
			button.ClearHover();
			button.StorageButton.Visible = false;
			if (entity == null)
			{
				button.SpriteView.Sprite = null;
				return;
			}
			HandVirtualItemComponent handVirtualItemComponent;
			SpriteComponent sprite;
			if (this._entMan.TryGetComponent<HandVirtualItemComponent>(entity, ref handVirtualItemComponent))
			{
				this._entMan.TryGetComponent<SpriteComponent>(handVirtualItemComponent.BlockingEntity, ref sprite);
			}
			else if (!this._entMan.TryGetComponent<SpriteComponent>(entity, ref sprite))
			{
				return;
			}
			button.SpriteView.Sprite = sprite;
		}

		// Token: 0x04000867 RID: 2151
		private const int ButtonSeparation = 4;

		// Token: 0x04000868 RID: 2152
		[Dependency]
		private readonly IPrototypeManager _protoMan;

		// Token: 0x04000869 RID: 2153
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400086A RID: 2154
		private ExamineSystem _examine;

		// Token: 0x0400086B RID: 2155
		private InventorySystem _inv;

		// Token: 0x0400086C RID: 2156
		[Nullable(2)]
		[ViewVariables]
		private StrippingMenu _strippingMenu;

		// Token: 0x0400086D RID: 2157
		public const string HiddenPocketEntityId = "StrippingHiddenEntity";

		// Token: 0x0400086E RID: 2158
		private EntityUid _virtualHiddenEntity;
	}
}
