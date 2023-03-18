using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C5 RID: 709
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class PathRouteMessage : EntityEventArgs
	{
		// Token: 0x060007CC RID: 1996 RVA: 0x0001A04E File Offset: 0x0001824E
		public PathRouteMessage(List<DebugPathPoly> path, Dictionary<DebugPathPoly, float> costs)
		{
			this.Path = path;
			this.Costs = costs;
		}

		// Token: 0x040007F6 RID: 2038
		public List<DebugPathPoly> Path;

		// Token: 0x040007F7 RID: 2039
		public Dictionary<DebugPathPoly, float> Costs;
	}
}
