using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B8 RID: 952
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class InventoryEquipActEvent : EntityEventArgs
	{
		// Token: 0x06000AFE RID: 2814 RVA: 0x00024490 File Offset: 0x00022690
		public InventoryEquipActEvent(EntityUid uid, EntityUid itemUid, string slot, bool silent = false, bool force = false)
		{
			this.Uid = uid;
			this.ItemUid = itemUid;
			this.Slot = slot;
			this.Silent = silent;
			this.Force = force;
		}

		// Token: 0x04000AEC RID: 2796
		public readonly EntityUid Uid;

		// Token: 0x04000AED RID: 2797
		public readonly EntityUid ItemUid;

		// Token: 0x04000AEE RID: 2798
		public readonly string Slot;

		// Token: 0x04000AEF RID: 2799
		public readonly bool Silent;

		// Token: 0x04000AF0 RID: 2800
		public readonly bool Force;
	}
}
