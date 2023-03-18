using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Explosion.EntitySystems
{
	// Token: 0x0200050D RID: 1293
	public sealed class BlockedSpaceTile
	{
		// Token: 0x04001126 RID: 4390
		public AtmosDirection UnblockedDirections = AtmosDirection.All;

		// Token: 0x04001127 RID: 4391
		[Nullable(1)]
		public List<BlockedSpaceTile.GridEdgeData> BlockingGridEdges = new List<BlockedSpaceTile.GridEdgeData>();

		// Token: 0x02000A07 RID: 2567
		public sealed class GridEdgeData
		{
			// Token: 0x06003420 RID: 13344 RVA: 0x00109F4A File Offset: 0x0010814A
			public GridEdgeData(Vector2i tile, EntityUid? grid, Vector2 center, Angle angle, float size)
			{
				this.Tile = tile;
				this.Grid = grid;
				this.Box = new Box2Rotated(Box2.CenteredAround(center, new ValueTuple<float, float>(size, size)), angle, center);
			}

			// Token: 0x0400231A RID: 8986
			public Vector2i Tile;

			// Token: 0x0400231B RID: 8987
			public EntityUid? Grid;

			// Token: 0x0400231C RID: 8988
			public Box2Rotated Box;
		}
	}
}
