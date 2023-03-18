using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Inventory.Controls
{
	// Token: 0x0200007A RID: 122
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class ItemSlotButtonContainer : ItemSlotUIContainer<SlotControl>
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600028F RID: 655 RVA: 0x00011039 File Offset: 0x0000F239
		// (set) Token: 0x06000290 RID: 656 RVA: 0x00011041 File Offset: 0x0000F241
		public string SlotGroup
		{
			get
			{
				return this._slotGroup;
			}
			set
			{
				this._inventoryController.RemoveSlotGroup(this.SlotGroup);
				this._slotGroup = value;
				this._inventoryController.RegisterSlotGroupContainer(this);
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00011068 File Offset: 0x0000F268
		public ItemSlotButtonContainer()
		{
			this._inventoryController = base.UserInterfaceManager.GetUIController<InventoryUIController>();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0001108C File Offset: 0x0000F28C
		~ItemSlotButtonContainer()
		{
			this._inventoryController.RemoveSlotGroup(this.SlotGroup);
		}

		// Token: 0x0400016E RID: 366
		private readonly InventoryUIController _inventoryController;

		// Token: 0x0400016F RID: 367
		private string _slotGroup = "";
	}
}
