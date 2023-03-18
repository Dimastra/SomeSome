using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x02000339 RID: 825
	public sealed class PathfindingRequestEvent : EntityEventArgs
	{
		// Token: 0x04000A42 RID: 2626
		public EntityCoordinates Start;

		// Token: 0x04000A43 RID: 2627
		public EntityCoordinates End;
	}
}
