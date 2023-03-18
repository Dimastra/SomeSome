using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage
{
	// Token: 0x0200012D RID: 301
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AnimateInsertingEntitiesEvent : EntityEventArgs
	{
		// Token: 0x06000370 RID: 880 RVA: 0x0000E9C0 File Offset: 0x0000CBC0
		public AnimateInsertingEntitiesEvent(EntityUid storage, List<EntityUid> storedEntities, List<EntityCoordinates> entityPositions)
		{
			this.Storage = storage;
			this.StoredEntities = storedEntities;
			this.EntityPositions = entityPositions;
		}

		// Token: 0x04000390 RID: 912
		public readonly EntityUid Storage;

		// Token: 0x04000391 RID: 913
		public readonly List<EntityUid> StoredEntities;

		// Token: 0x04000392 RID: 914
		public readonly List<EntityCoordinates> EntityPositions;
	}
}
