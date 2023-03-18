using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033C RID: 828
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PathPoly : IEquatable<PathPoly>
	{
		// Token: 0x06001176 RID: 4470 RVA: 0x0005C30A File Offset: 0x0005A50A
		public PathPoly(EntityUid graphUid, Vector2i chunkOrigin, byte tileIndex, Box2 vertices, PathfindingData data, HashSet<PathPoly> neighbors)
		{
			this.GraphUid = graphUid;
			this.ChunkOrigin = chunkOrigin;
			this.TileIndex = tileIndex;
			this.Box = vertices;
			this.Data = data;
			this.Neighbors = neighbors;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0005C33F File Offset: 0x0005A53F
		public bool IsValid()
		{
			return (this.Data.Flags & PathfindingBreadcrumbFlag.Invalid) == PathfindingBreadcrumbFlag.None;
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x0005C351 File Offset: 0x0005A551
		[ViewVariables]
		public EntityCoordinates Coordinates
		{
			get
			{
				return new EntityCoordinates(this.GraphUid, this.Box.Center);
			}
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0005C36C File Offset: 0x0005A56C
		public bool IsEquivalent(PathPoly other)
		{
			return this.GraphUid.Equals(other.GraphUid) && this.ChunkOrigin.Equals(other.ChunkOrigin) && this.TileIndex == other.TileIndex && this.Data.IsEquivalent(other.Data) && this.Box.Equals(other.Box);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0005C3D4 File Offset: 0x0005A5D4
		[NullableContext(2)]
		public bool Equals(PathPoly other)
		{
			return other != null && this.GraphUid.Equals(other.GraphUid) && this.ChunkOrigin.Equals(other.ChunkOrigin) && this.TileIndex == other.TileIndex && this.Data.Equals(other.Data) && this.Box.Equals(other.Box);
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0005C440 File Offset: 0x0005A640
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (this != obj)
			{
				PathPoly other = obj as PathPoly;
				return other != null && this.Equals(other);
			}
			return true;
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x0005C466 File Offset: 0x0005A666
		public override int GetHashCode()
		{
			return HashCode.Combine<EntityUid, Vector2i, byte, Box2>(this.GraphUid, this.ChunkOrigin, this.TileIndex, this.Box);
		}

		// Token: 0x04000A60 RID: 2656
		[ViewVariables]
		public readonly EntityUid GraphUid;

		// Token: 0x04000A61 RID: 2657
		[ViewVariables]
		public readonly Vector2i ChunkOrigin;

		// Token: 0x04000A62 RID: 2658
		[ViewVariables]
		public readonly byte TileIndex;

		// Token: 0x04000A63 RID: 2659
		[ViewVariables]
		public readonly Box2 Box;

		// Token: 0x04000A64 RID: 2660
		[ViewVariables]
		public PathfindingData Data;

		// Token: 0x04000A65 RID: 2661
		[ViewVariables]
		public readonly HashSet<PathPoly> Neighbors;
	}
}
