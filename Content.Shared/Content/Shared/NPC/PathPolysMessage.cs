using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C3 RID: 707
	[NetSerializable]
	[Serializable]
	public sealed class PathPolysMessage : EntityEventArgs
	{
		// Token: 0x040007F2 RID: 2034
		[Nullable(1)]
		public Dictionary<EntityUid, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>> Polys = new Dictionary<EntityUid, Dictionary<Vector2i, Dictionary<Vector2i, List<DebugPathPoly>>>>();
	}
}
