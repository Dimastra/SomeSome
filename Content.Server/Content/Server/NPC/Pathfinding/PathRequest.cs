using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033E RID: 830
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class PathRequest
	{
		// Token: 0x17000278 RID: 632
		// (get) Token: 0x0600117F RID: 4479 RVA: 0x0005C4A4 File Offset: 0x0005A6A4
		public Task<PathResult> Task
		{
			get
			{
				return this.Tcs.Task;
			}
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x0005C4B4 File Offset: 0x0005A6B4
		public PathRequest(EntityCoordinates start, PathFlags flags, int layer, int mask, CancellationToken cancelToken)
		{
			this.Start = start;
			this.Flags = flags;
			this.CollisionLayer = layer;
			this.CollisionMask = mask;
			this.Tcs = new TaskCompletionSource<PathResult>(cancelToken);
		}

		// Token: 0x04000A69 RID: 2665
		public EntityCoordinates Start;

		// Token: 0x04000A6A RID: 2666
		public readonly TaskCompletionSource<PathResult> Tcs;

		// Token: 0x04000A6B RID: 2667
		public Queue<PathPoly> Polys = new Queue<PathPoly>();

		// Token: 0x04000A6C RID: 2668
		public bool Started;

		// Token: 0x04000A6D RID: 2669
		public readonly Stopwatch Stopwatch = new Stopwatch();

		// Token: 0x04000A6E RID: 2670
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public PriorityQueue<ValueTuple<float, PathPoly>> Frontier;

		// Token: 0x04000A6F RID: 2671
		public readonly Dictionary<PathPoly, float> CostSoFar = new Dictionary<PathPoly, float>();

		// Token: 0x04000A70 RID: 2672
		public readonly Dictionary<PathPoly, PathPoly> CameFrom = new Dictionary<PathPoly, PathPoly>();

		// Token: 0x04000A71 RID: 2673
		public readonly PathFlags Flags;

		// Token: 0x04000A72 RID: 2674
		public readonly int CollisionLayer;

		// Token: 0x04000A73 RID: 2675
		public readonly int CollisionMask;
	}
}
