using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Payload.Components
{
	// Token: 0x02000297 RID: 663
	[Flags]
	[NetSerializable]
	[Serializable]
	public enum ChemicalPayloadFilledSlots : byte
	{
		// Token: 0x0400078A RID: 1930
		None = 0,
		// Token: 0x0400078B RID: 1931
		Left = 1,
		// Token: 0x0400078C RID: 1932
		Right = 2,
		// Token: 0x0400078D RID: 1933
		Both = 3
	}
}
