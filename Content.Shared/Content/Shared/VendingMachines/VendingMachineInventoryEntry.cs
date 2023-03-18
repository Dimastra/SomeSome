using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines
{
	// Token: 0x02000095 RID: 149
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VendingMachineInventoryEntry
	{
		// Token: 0x060001BF RID: 447 RVA: 0x00009AE3 File Offset: 0x00007CE3
		public VendingMachineInventoryEntry(InventoryType type, string id, uint amount)
		{
			this.Type = type;
			this.ID = id;
			this.Amount = amount;
		}

		// Token: 0x04000219 RID: 537
		[ViewVariables]
		public InventoryType Type;

		// Token: 0x0400021A RID: 538
		[ViewVariables]
		public string ID;

		// Token: 0x0400021B RID: 539
		[ViewVariables]
		public uint Amount;
	}
}
