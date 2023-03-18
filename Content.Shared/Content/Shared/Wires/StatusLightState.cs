using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000022 RID: 34
	[NetSerializable]
	[Serializable]
	public enum StatusLightState : byte
	{
		// Token: 0x04000044 RID: 68
		Off,
		// Token: 0x04000045 RID: 69
		On,
		// Token: 0x04000046 RID: 70
		BlinkingFast,
		// Token: 0x04000047 RID: 71
		BlinkingSlow
	}
}
