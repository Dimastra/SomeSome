using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Revenant
{
	// Token: 0x020001F5 RID: 501
	[NetSerializable]
	[Serializable]
	public enum RevenantVisuals : byte
	{
		// Token: 0x0400059A RID: 1434
		Corporeal,
		// Token: 0x0400059B RID: 1435
		Stunned,
		// Token: 0x0400059C RID: 1436
		Harvesting
	}
}
