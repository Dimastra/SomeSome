using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals
{
	// Token: 0x0200069D RID: 1693
	[NetSerializable]
	[Serializable]
	public enum PortableScrubberVisuals : byte
	{
		// Token: 0x040014B1 RID: 5297
		IsFull,
		// Token: 0x040014B2 RID: 5298
		IsRunning,
		// Token: 0x040014B3 RID: 5299
		IsDraining
	}
}
