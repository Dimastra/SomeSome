using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A3 RID: 419
	[NetSerializable]
	[Serializable]
	public enum PowerLevelVisuals : byte
	{
		// Token: 0x040004A3 RID: 1187
		NoPower,
		// Token: 0x040004A4 RID: 1188
		LowPower,
		// Token: 0x040004A5 RID: 1189
		MediumPower,
		// Token: 0x040004A6 RID: 1190
		HighPower
	}
}
