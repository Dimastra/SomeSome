using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x02000341 RID: 833
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PathResultEvent
	{
		// Token: 0x06001183 RID: 4483 RVA: 0x0005C560 File Offset: 0x0005A760
		public PathResultEvent(PathResult result, Queue<PathPoly> path)
		{
			this.Result = result;
			this.Path = path;
		}

		// Token: 0x04000A78 RID: 2680
		public PathResult Result;

		// Token: 0x04000A79 RID: 2681
		public readonly Queue<PathPoly> Path;
	}
}
