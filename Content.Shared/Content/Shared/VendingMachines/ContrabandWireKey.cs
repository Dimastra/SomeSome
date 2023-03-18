using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
	// Token: 0x0200009A RID: 154
	[NetSerializable]
	[Serializable]
	public enum ContrabandWireKey : byte
	{
		// Token: 0x0400022D RID: 557
		StatusKey,
		// Token: 0x0400022E RID: 558
		TimeoutKey
	}
}
