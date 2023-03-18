using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids
{
	// Token: 0x02000487 RID: 1159
	[NetSerializable]
	[Serializable]
	public readonly struct PuddleDebugOverlayData
	{
		// Token: 0x06000DF3 RID: 3571 RVA: 0x0002D6DC File Offset: 0x0002B8DC
		public PuddleDebugOverlayData(Vector2i pos, FixedPoint2 currentVolume)
		{
			this.CurrentVolume = currentVolume;
			this.Pos = pos;
		}

		// Token: 0x04000D49 RID: 3401
		public readonly Vector2i Pos;

		// Token: 0x04000D4A RID: 3402
		public readonly FixedPoint2 CurrentVolume;
	}
}
