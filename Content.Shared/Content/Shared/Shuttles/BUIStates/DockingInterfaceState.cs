using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates
{
	// Token: 0x020001D5 RID: 469
	[NetSerializable]
	[Serializable]
	public sealed class DockingInterfaceState
	{
		// Token: 0x04000548 RID: 1352
		public EntityCoordinates Coordinates;

		// Token: 0x04000549 RID: 1353
		public Angle Angle;

		// Token: 0x0400054A RID: 1354
		public EntityUid Entity;

		// Token: 0x0400054B RID: 1355
		public bool Connected;

		// Token: 0x0400054C RID: 1356
		public Color Color;

		// Token: 0x0400054D RID: 1357
		public Color HighlightedColor;
	}
}
