using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C2 RID: 706
	[NetSerializable]
	[Serializable]
	public sealed class PathBreadcrumbsRefreshMessage : EntityEventArgs
	{
		// Token: 0x040007EF RID: 2031
		public EntityUid GridUid;

		// Token: 0x040007F0 RID: 2032
		public Vector2i Origin;

		// Token: 0x040007F1 RID: 2033
		[Nullable(1)]
		public List<PathfindingBreadcrumb> Data = new List<PathfindingBreadcrumb>();
	}
}
