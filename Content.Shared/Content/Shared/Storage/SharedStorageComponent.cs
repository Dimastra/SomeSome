using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage
{
	// Token: 0x0200012C RID: 300
	[NetworkedComponent]
	public abstract class SharedStorageComponent : Component
	{
		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600036D RID: 877
		[Nullable(2)]
		public abstract IReadOnlyList<EntityUid> StoredEntities { [NullableContext(2)] get; }

		// Token: 0x0600036E RID: 878
		public abstract bool Remove(EntityUid entity);

		// Token: 0x0200079A RID: 1946
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class StorageBoundUserInterfaceState : BoundUserInterfaceState
		{
			// Token: 0x060017DA RID: 6106 RVA: 0x0004D21A File Offset: 0x0004B41A
			public StorageBoundUserInterfaceState(List<EntityUid> storedEntities, int storageSizeUsed, int storageCapacityMax)
			{
				this.StoredEntities = storedEntities;
				this.StorageSizeUsed = storageSizeUsed;
				this.StorageCapacityMax = storageCapacityMax;
			}

			// Token: 0x040017B2 RID: 6066
			public readonly List<EntityUid> StoredEntities;

			// Token: 0x040017B3 RID: 6067
			public readonly int StorageSizeUsed;

			// Token: 0x040017B4 RID: 6068
			public readonly int StorageCapacityMax;
		}

		// Token: 0x0200079B RID: 1947
		[NetSerializable]
		[Serializable]
		public sealed class StorageInsertItemMessage : BoundUserInterfaceMessage
		{
		}

		// Token: 0x0200079C RID: 1948
		[NetSerializable]
		[Serializable]
		public sealed class StorageInteractWithItemEvent : BoundUserInterfaceMessage
		{
			// Token: 0x060017DC RID: 6108 RVA: 0x0004D23F File Offset: 0x0004B43F
			public StorageInteractWithItemEvent(EntityUid interactedItemUID)
			{
				this.InteractedItemUID = interactedItemUID;
			}

			// Token: 0x040017B5 RID: 6069
			public readonly EntityUid InteractedItemUID;
		}

		// Token: 0x0200079D RID: 1949
		[NetSerializable]
		[Serializable]
		public enum StorageUiKey
		{
			// Token: 0x040017B7 RID: 6071
			Key
		}
	}
}
