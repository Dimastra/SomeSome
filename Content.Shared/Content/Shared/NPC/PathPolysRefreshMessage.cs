using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C4 RID: 708
	[NetSerializable]
	[Serializable]
	public sealed class PathPolysRefreshMessage : EntityEventArgs
	{
		// Token: 0x040007F3 RID: 2035
		public EntityUid GridUid;

		// Token: 0x040007F4 RID: 2036
		public Vector2i Origin;

		// Token: 0x040007F5 RID: 2037
		[Nullable(1)]
		public Dictionary<Vector2i, List<DebugPathPoly>> Polys = new Dictionary<Vector2i, List<DebugPathPoly>>();
	}
}
