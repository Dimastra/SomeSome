using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Light
{
	// Token: 0x02000363 RID: 867
	[NetSerializable]
	[Serializable]
	public enum HandheldLightPowerStates
	{
		// Token: 0x040009F0 RID: 2544
		FullPower,
		// Token: 0x040009F1 RID: 2545
		LowPower,
		// Token: 0x040009F2 RID: 2546
		Dying
	}
}
