using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C1 RID: 705
	[NetSerializable]
	[Serializable]
	public sealed class PathBreadcrumbsMessage : EntityEventArgs
	{
		// Token: 0x040007EE RID: 2030
		[Nullable(1)]
		public Dictionary<EntityUid, Dictionary<Vector2i, List<PathfindingBreadcrumb>>> Breadcrumbs = new Dictionary<EntityUid, Dictionary<Vector2i, List<PathfindingBreadcrumb>>>();
	}
}
