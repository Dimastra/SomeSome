using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
	// Token: 0x0200009D RID: 157
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VendingMachineInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00009B08 File Offset: 0x00007D08
		public VendingMachineInterfaceState(List<VendingMachineInventoryEntry> inventory)
		{
			this.Inventory = inventory;
		}

		// Token: 0x04000231 RID: 561
		public List<VendingMachineInventoryEntry> Inventory;
	}
}
