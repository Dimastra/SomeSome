using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly
{
	// Token: 0x020006F9 RID: 1785
	[NetSerializable]
	[Serializable]
	public enum AnomalousParticleType : byte
	{
		// Token: 0x040015BB RID: 5563
		Delta,
		// Token: 0x040015BC RID: 5564
		Epsilon,
		// Token: 0x040015BD RID: 5565
		Zeta
	}
}
