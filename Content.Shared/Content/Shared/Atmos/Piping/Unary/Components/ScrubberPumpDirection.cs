using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
	// Token: 0x020006B1 RID: 1713
	[NetSerializable]
	[Serializable]
	public enum ScrubberPumpDirection : sbyte
	{
		// Token: 0x040014F8 RID: 5368
		Siphoning,
		// Token: 0x040014F9 RID: 5369
		Scrubbing
	}
}
