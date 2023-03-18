using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
	// Token: 0x0200009E RID: 158
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VendingMachineEjectMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060001C2 RID: 450 RVA: 0x00009B17 File Offset: 0x00007D17
		public VendingMachineEjectMessage(InventoryType type, string id)
		{
			this.Type = type;
			this.ID = id;
		}

		// Token: 0x04000232 RID: 562
		public readonly InventoryType Type;

		// Token: 0x04000233 RID: 563
		public readonly string ID;
	}
}
