using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Explosion
{
	// Token: 0x020004A5 RID: 1189
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	public abstract class SharedExplosionVisualsComponent : Component
	{
		// Token: 0x04000D8B RID: 3467
		public MapCoordinates Epicenter;

		// Token: 0x04000D8C RID: 3468
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Dictionary<int, List<Vector2i>> SpaceTiles;

		// Token: 0x04000D8D RID: 3469
		public Dictionary<EntityUid, Dictionary<int, List<Vector2i>>> Tiles = new Dictionary<EntityUid, Dictionary<int, List<Vector2i>>>();

		// Token: 0x04000D8E RID: 3470
		public List<float> Intensity = new List<float>();

		// Token: 0x04000D8F RID: 3471
		public string ExplosionType = string.Empty;

		// Token: 0x04000D90 RID: 3472
		public Matrix3 SpaceMatrix;

		// Token: 0x04000D91 RID: 3473
		public ushort SpaceTileSize;
	}
}
