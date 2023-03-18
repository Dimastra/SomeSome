using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
	// Token: 0x02000096 RID: 150
	[NetSerializable]
	[Serializable]
	public enum InventoryType : byte
	{
		// Token: 0x0400021D RID: 541
		Regular,
		// Token: 0x0400021E RID: 542
		Emagged,
		// Token: 0x0400021F RID: 543
		Contraband
	}
}
