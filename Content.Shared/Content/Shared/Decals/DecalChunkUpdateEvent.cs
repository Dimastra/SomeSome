using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals
{
	// Token: 0x02000523 RID: 1315
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DecalChunkUpdateEvent : EntityEventArgs
	{
		// Token: 0x04000F16 RID: 3862
		public Dictionary<EntityUid, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> Data = new Dictionary<EntityUid, Dictionary<Vector2i, DecalGridComponent.DecalChunk>>();

		// Token: 0x04000F17 RID: 3863
		public Dictionary<EntityUid, HashSet<Vector2i>> RemovedChunks = new Dictionary<EntityUid, HashSet<Vector2i>>();
	}
}
