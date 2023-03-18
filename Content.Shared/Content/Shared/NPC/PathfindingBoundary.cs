using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C7 RID: 711
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public struct PathfindingBoundary
	{
		// Token: 0x060007CE RID: 1998 RVA: 0x0001A06C File Offset: 0x0001826C
		public PathfindingBoundary(bool closed, List<PathfindingBreadcrumb> crumbs)
		{
			this.Closed = closed;
			this.Breadcrumbs = crumbs;
		}

		// Token: 0x040007F9 RID: 2041
		public List<PathfindingBreadcrumb> Breadcrumbs;

		// Token: 0x040007FA RID: 2042
		public bool Closed;
	}
}
