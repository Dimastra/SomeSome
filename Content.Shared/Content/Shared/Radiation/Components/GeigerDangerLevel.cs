using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Components
{
	// Token: 0x0200022F RID: 559
	[NetSerializable]
	[Serializable]
	public enum GeigerDangerLevel : byte
	{
		// Token: 0x04000640 RID: 1600
		None,
		// Token: 0x04000641 RID: 1601
		Low,
		// Token: 0x04000642 RID: 1602
		Med,
		// Token: 0x04000643 RID: 1603
		High,
		// Token: 0x04000644 RID: 1604
		Extreme
	}
}
