using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.NPC;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x02000337 RID: 823
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GridPathfindingChunk
	{
		// Token: 0x0600112F RID: 4399 RVA: 0x000592E0 File Offset: 0x000574E0
		public GridPathfindingChunk()
		{
			for (int x = 0; x < this.Polygons.Length; x++)
			{
				this.Polygons[x] = new List<PathPoly>();
				this.BufferPolygons[x] = new List<PathPoly>();
			}
		}

		// Token: 0x04000A37 RID: 2615
		[ViewVariables]
		public readonly PathfindingBreadcrumb[,] Points = new PathfindingBreadcrumb[32, 32];

		// Token: 0x04000A38 RID: 2616
		[ViewVariables]
		public Vector2i Origin;

		// Token: 0x04000A39 RID: 2617
		[ViewVariables]
		public readonly List<PathPoly>[] Polygons = new List<PathPoly>[64];

		// Token: 0x04000A3A RID: 2618
		internal readonly List<PathPoly>[] BufferPolygons = new List<PathPoly>[64];

		// Token: 0x04000A3B RID: 2619
		[ViewVariables]
		public readonly Dictionary<PathPortal, PathPoly> PortalPolys = new Dictionary<PathPortal, PathPoly>();

		// Token: 0x04000A3C RID: 2620
		[ViewVariables]
		public readonly List<PathPortal> Portals = new List<PathPortal>();
	}
}
