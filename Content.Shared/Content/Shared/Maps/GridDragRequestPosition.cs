using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Maps
{
	// Token: 0x0200033C RID: 828
	[NetSerializable]
	[Serializable]
	public sealed class GridDragRequestPosition : EntityEventArgs
	{
		// Token: 0x0400097D RID: 2429
		public EntityUid Grid;

		// Token: 0x0400097E RID: 2430
		public Vector2 WorldPosition;
	}
}
