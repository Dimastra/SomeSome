using System;
using System.Runtime.CompilerServices;
using Content.Client.Inventory;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E3 RID: 227
	public sealed class SlotButton : SlotControl
	{
		// Token: 0x06000659 RID: 1625 RVA: 0x00021E64 File Offset: 0x00020064
		public SlotButton()
		{
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00021E6C File Offset: 0x0002006C
		[NullableContext(1)]
		public SlotButton(ClientInventorySystem.SlotData slotData)
		{
			base.ButtonTexturePath = slotData.TextureName;
			base.Blocked = slotData.Blocked;
			base.Highlight = slotData.Highlighted;
			base.StorageTexturePath = "Slots/back";
			base.SlotName = slotData.SlotName;
		}
	}
}
