using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
	// Token: 0x02000257 RID: 599
	[NetSerializable]
	[Serializable]
	public enum CellChargerStatus
	{
		// Token: 0x040006C1 RID: 1729
		Off,
		// Token: 0x040006C2 RID: 1730
		Empty,
		// Token: 0x040006C3 RID: 1731
		Charging,
		// Token: 0x040006C4 RID: 1732
		Charged
	}
}
