using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x02000338 RID: 824
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(PathfindingSystem)
	})]
	public sealed class GridPathfindingComponent : Component
	{
		// Token: 0x04000A3D RID: 2621
		[ViewVariables]
		public readonly HashSet<Vector2i> DirtyChunks = new HashSet<Vector2i>();

		// Token: 0x04000A3E RID: 2622
		[ViewVariables]
		[DataField("nextUpdate", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextUpdate;

		// Token: 0x04000A3F RID: 2623
		[ViewVariables]
		public readonly Dictionary<Vector2i, GridPathfindingChunk> Chunks = new Dictionary<Vector2i, GridPathfindingChunk>();

		// Token: 0x04000A40 RID: 2624
		[ViewVariables]
		public readonly Dictionary<PathPortal, Vector2i> PortalLookup = new Dictionary<PathPortal, Vector2i>();

		// Token: 0x04000A41 RID: 2625
		[ViewVariables]
		public readonly List<PathPortal> DirtyPortals = new List<PathPortal>();
	}
}
