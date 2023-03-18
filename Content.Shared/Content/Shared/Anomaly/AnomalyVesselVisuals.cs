using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly
{
	// Token: 0x020006FA RID: 1786
	[NetSerializable]
	[Serializable]
	public enum AnomalyVesselVisuals : byte
	{
		// Token: 0x040015BF RID: 5567
		HasAnomaly,
		// Token: 0x040015C0 RID: 5568
		AnomalyState
	}
}
