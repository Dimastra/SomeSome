using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Placeable
{
	// Token: 0x02000271 RID: 625
	[NetSerializable]
	[Serializable]
	public sealed class PlaceableSurfaceComponentState : ComponentState
	{
		// Token: 0x06000726 RID: 1830 RVA: 0x00018701 File Offset: 0x00016901
		public PlaceableSurfaceComponentState(bool placeable, bool centered, Vector2 offset)
		{
			this.IsPlaceable = placeable;
			this.PlaceCentered = centered;
			this.PositionOffset = offset;
		}

		// Token: 0x04000707 RID: 1799
		public readonly bool IsPlaceable;

		// Token: 0x04000708 RID: 1800
		public readonly bool PlaceCentered;

		// Token: 0x04000709 RID: 1801
		public readonly Vector2 PositionOffset;
	}
}
