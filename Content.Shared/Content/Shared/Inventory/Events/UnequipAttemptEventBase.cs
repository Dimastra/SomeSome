using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BA RID: 954
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class UnequipAttemptEventBase : CancellableEntityEventArgs
	{
		// Token: 0x06000B00 RID: 2816 RVA: 0x000244CC File Offset: 0x000226CC
		public UnequipAttemptEventBase(EntityUid unequipee, EntityUid unEquipTarget, EntityUid equipment, SlotDefinition slotDefinition)
		{
			this.UnEquipTarget = unEquipTarget;
			this.Equipment = equipment;
			this.Unequipee = unequipee;
			this.Slot = slotDefinition.Name;
		}

		// Token: 0x04000AF2 RID: 2802
		public readonly EntityUid Unequipee;

		// Token: 0x04000AF3 RID: 2803
		public readonly EntityUid UnEquipTarget;

		// Token: 0x04000AF4 RID: 2804
		public readonly EntityUid Equipment;

		// Token: 0x04000AF5 RID: 2805
		public readonly string Slot;

		// Token: 0x04000AF6 RID: 2806
		[Nullable(2)]
		public string Reason;
	}
}
