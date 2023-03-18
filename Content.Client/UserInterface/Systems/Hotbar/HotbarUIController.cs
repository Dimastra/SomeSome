using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.Hands;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Client.UserInterface.Systems.Inventory;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems.Hotbar
{
	// Token: 0x0200007F RID: 127
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HotbarUIController : UIController
	{
		// Token: 0x060002B2 RID: 690 RVA: 0x0001195E File Offset: 0x0000FB5E
		public void Setup(HandsContainer handsContainer, ItemSlotButtonContainer inventoryBar, ItemStatusPanel handStatus)
		{
			this._inventory = this.UIManager.GetUIController<InventoryUIController>();
			this._hands = this.UIManager.GetUIController<HandsUIController>();
			this._hands.RegisterHandContainer(handsContainer);
			this._inventory.RegisterInventoryBarContainer(inventoryBar);
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0001199C File Offset: 0x0000FB9C
		public void ReloadHotbar()
		{
			if (this.UIManager.ActiveScreen == null)
			{
				return;
			}
			HotbarGui widget = this.UIManager.ActiveScreen.GetWidget<HotbarGui>();
			if (widget == null)
			{
				return;
			}
			foreach (ItemSlotButtonContainer itemSlotButtonContainer in this.GetAllItemSlotContainers(widget))
			{
				itemSlotButtonContainer.SlotGroup = itemSlotButtonContainer.SlotGroup;
			}
			HandsUIController hands = this._hands;
			if (hands != null)
			{
				hands.ReloadHands();
			}
			InventoryUIController inventory = this._inventory;
			if (inventory != null)
			{
				inventory.ReloadSlots();
			}
			InventoryUIController inventory2 = this._inventory;
			if (inventory2 == null)
			{
				return;
			}
			inventory2.RegisterInventoryBarContainer(widget.InventoryHotbar);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00011A48 File Offset: 0x0000FC48
		private IEnumerable<ItemSlotButtonContainer> GetAllItemSlotContainers(Control gui)
		{
			List<ItemSlotButtonContainer> list = new List<ItemSlotButtonContainer>();
			foreach (Control control in gui.Children)
			{
				ItemSlotButtonContainer itemSlotButtonContainer = control as ItemSlotButtonContainer;
				if (itemSlotButtonContainer != null)
				{
					list.Add(itemSlotButtonContainer);
				}
				list.AddRange(this.GetAllItemSlotContainers(control));
			}
			return list;
		}

		// Token: 0x04000174 RID: 372
		[Nullable(2)]
		private InventoryUIController _inventory;

		// Token: 0x04000175 RID: 373
		[Nullable(2)]
		private HandsUIController _hands;
	}
}
