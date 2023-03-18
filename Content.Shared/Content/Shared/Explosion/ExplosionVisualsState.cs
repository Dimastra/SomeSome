using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Explosion
{
	// Token: 0x020004A6 RID: 1190
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ExplosionVisualsState : ComponentState
	{
		// Token: 0x06000E64 RID: 3684 RVA: 0x0002E3B8 File Offset: 0x0002C5B8
		public ExplosionVisualsState(MapCoordinates epicenter, string typeID, List<float> intensity, [Nullable(new byte[]
		{
			2,
			1
		})] Dictionary<int, List<Vector2i>> spaceTiles, Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> tiles, Matrix3 spaceMatrix, ushort spaceTileSize)
		{
			this.Epicenter = epicenter;
			this.SpaceTiles = spaceTiles;
			this.Tiles = tiles;
			this.Intensity = intensity;
			this.ExplosionType = typeID;
			this.SpaceMatrix = spaceMatrix;
			this.SpaceTileSize = spaceTileSize;
		}

		// Token: 0x04000D92 RID: 3474
		public MapCoordinates Epicenter;

		// Token: 0x04000D93 RID: 3475
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Dictionary<int, List<Vector2i>> SpaceTiles;

		// Token: 0x04000D94 RID: 3476
		public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles;

		// Token: 0x04000D95 RID: 3477
		public List<float> Intensity;

		// Token: 0x04000D96 RID: 3478
		public string ExplosionType = string.Empty;

		// Token: 0x04000D97 RID: 3479
		public Matrix3 SpaceMatrix;

		// Token: 0x04000D98 RID: 3480
		public ushort SpaceTileSize;
	}
}
