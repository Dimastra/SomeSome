using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Visuals
{
	// Token: 0x020006A6 RID: 1702
	[NetSerializable]
	[Serializable]
	public enum ScrubberState : byte
	{
		// Token: 0x040014C5 RID: 5317
		Off,
		// Token: 0x040014C6 RID: 5318
		Scrub,
		// Token: 0x040014C7 RID: 5319
		Siphon,
		// Token: 0x040014C8 RID: 5320
		WideScrub,
		// Token: 0x040014C9 RID: 5321
		Welded
	}
}
