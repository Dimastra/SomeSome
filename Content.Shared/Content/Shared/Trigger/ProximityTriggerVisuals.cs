using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Trigger
{
	// Token: 0x020000A9 RID: 169
	[NetSerializable]
	[Serializable]
	public enum ProximityTriggerVisuals : byte
	{
		// Token: 0x04000257 RID: 599
		Off,
		// Token: 0x04000258 RID: 600
		Inactive,
		// Token: 0x04000259 RID: 601
		Active
	}
}
