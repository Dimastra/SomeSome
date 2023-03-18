using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000021 RID: 33
	[NetSerializable]
	[Serializable]
	public enum WiresAction : byte
	{
		// Token: 0x04000040 RID: 64
		Mend,
		// Token: 0x04000041 RID: 65
		Cut,
		// Token: 0x04000042 RID: 66
		Pulse
	}
}
