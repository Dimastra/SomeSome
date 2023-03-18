using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002CC RID: 716
	[NetSerializable]
	[Serializable]
	public sealed class DebugPathPoly
	{
		// Token: 0x04000813 RID: 2067
		public EntityUid GraphUid;

		// Token: 0x04000814 RID: 2068
		public Vector2i ChunkOrigin;

		// Token: 0x04000815 RID: 2069
		public byte TileIndex;

		// Token: 0x04000816 RID: 2070
		public Box2 Box;

		// Token: 0x04000817 RID: 2071
		public PathfindingData Data;

		// Token: 0x04000818 RID: 2072
		[Nullable(1)]
		public List<EntityCoordinates> Neighbors;
	}
}
