using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B2 RID: 946
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class EquipAttemptBase : CancellableEntityEventArgs
	{
		// Token: 0x06000AF8 RID: 2808 RVA: 0x000243EF File Offset: 0x000225EF
		public EquipAttemptBase(EntityUid equipee, EntityUid equipTarget, EntityUid equipment, SlotDefinition slotDefinition)
		{
			this.EquipTarget = equipTarget;
			this.Equipment = equipment;
			this.Equipee = equipee;
			this.SlotFlags = slotDefinition.SlotFlags;
			this.Slot = slotDefinition.Name;
		}

		// Token: 0x04000AE1 RID: 2785
		public readonly EntityUid Equipee;

		// Token: 0x04000AE2 RID: 2786
		public readonly EntityUid EquipTarget;

		// Token: 0x04000AE3 RID: 2787
		public readonly EntityUid Equipment;

		// Token: 0x04000AE4 RID: 2788
		public readonly SlotFlags SlotFlags;

		// Token: 0x04000AE5 RID: 2789
		public readonly string Slot;

		// Token: 0x04000AE6 RID: 2790
		[Nullable(2)]
		public string Reason;
	}
}
