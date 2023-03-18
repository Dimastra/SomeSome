using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids
{
	// Token: 0x02000482 RID: 1154
	[NetSerializable]
	[Serializable]
	public enum PuddleVisuals : byte
	{
		// Token: 0x04000D40 RID: 3392
		VolumeScale,
		// Token: 0x04000D41 RID: 3393
		CurrentVolume,
		// Token: 0x04000D42 RID: 3394
		SolutionColor,
		// Token: 0x04000D43 RID: 3395
		IsEvaporatingVisual
	}
}
