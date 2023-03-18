using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Maps
{
	// Token: 0x0200033D RID: 829
	[NetSerializable]
	[Serializable]
	public sealed class GridDragVelocityRequest : EntityEventArgs
	{
		// Token: 0x0400097F RID: 2431
		public EntityUid Grid;

		// Token: 0x04000980 RID: 2432
		public Vector2 LinearVelocity;
	}
}
