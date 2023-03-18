using System;
using System.Threading;
using Robust.Shared.Map;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x02000340 RID: 832
	public sealed class BFSPathRequest : PathRequest
	{
		// Token: 0x06001182 RID: 4482 RVA: 0x0005C541 File Offset: 0x0005A741
		public BFSPathRequest(float expansionRange, int expansionLimit, EntityCoordinates start, PathFlags flags, int layer, int mask, CancellationToken cancelToken) : base(start, flags, layer, mask, cancelToken)
		{
			this.ExpansionRange = expansionRange;
			this.ExpansionLimit = expansionLimit;
		}

		// Token: 0x04000A76 RID: 2678
		public float ExpansionRange;

		// Token: 0x04000A77 RID: 2679
		public int ExpansionLimit;
	}
}
