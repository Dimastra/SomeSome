using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B9 RID: 953
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class OpenSlotStorageNetworkMessage : EntityEventArgs
	{
		// Token: 0x06000AFF RID: 2815 RVA: 0x000244BD File Offset: 0x000226BD
		public OpenSlotStorageNetworkMessage(string slot)
		{
			this.Slot = slot;
		}

		// Token: 0x04000AF1 RID: 2801
		public readonly string Slot;
	}
}
