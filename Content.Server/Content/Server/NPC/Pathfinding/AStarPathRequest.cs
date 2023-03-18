using System;
using System.Threading;
using Robust.Shared.Map;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033F RID: 831
	public sealed class AStarPathRequest : PathRequest
	{
		// Token: 0x06001181 RID: 4481 RVA: 0x0005C522 File Offset: 0x0005A722
		public AStarPathRequest(EntityCoordinates start, EntityCoordinates end, PathFlags flags, float distance, int layer, int mask, CancellationToken cancelToken) : base(start, flags, layer, mask, cancelToken)
		{
			this.Distance = distance;
			this.End = end;
		}

		// Token: 0x04000A74 RID: 2676
		public EntityCoordinates End;

		// Token: 0x04000A75 RID: 2677
		public float Distance;
	}
}
