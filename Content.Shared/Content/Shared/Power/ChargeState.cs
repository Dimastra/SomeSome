using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Power
{
	// Token: 0x02000254 RID: 596
	[NetSerializable]
	[Serializable]
	public enum ChargeState
	{
		// Token: 0x040006B0 RID: 1712
		Still,
		// Token: 0x040006B1 RID: 1713
		Charging,
		// Token: 0x040006B2 RID: 1714
		Discharging
	}
}
