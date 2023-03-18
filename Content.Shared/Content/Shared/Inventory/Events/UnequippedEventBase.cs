using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BD RID: 957
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class UnequippedEventBase : EntityEventArgs
	{
		// Token: 0x06000B03 RID: 2819 RVA: 0x00024510 File Offset: 0x00022710
		public UnequippedEventBase(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition)
		{
			this.Equipee = equipee;
			this.Equipment = equipment;
			this.Slot = slotDefinition.Name;
			this.SlotGroup = slotDefinition.SlotGroup;
		}

		// Token: 0x04000AF7 RID: 2807
		public readonly EntityUid Equipee;

		// Token: 0x04000AF8 RID: 2808
		public readonly EntityUid Equipment;

		// Token: 0x04000AF9 RID: 2809
		public readonly string Slot;

		// Token: 0x04000AFA RID: 2810
		public readonly string SlotGroup;
	}
}
