using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B5 RID: 949
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class EquippedEventBase : EntityEventArgs
	{
		// Token: 0x06000AFB RID: 2811 RVA: 0x00024440 File Offset: 0x00022640
		public EquippedEventBase(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition)
		{
			this.Equipee = equipee;
			this.Equipment = equipment;
			this.Slot = slotDefinition.Name;
			this.SlotGroup = slotDefinition.SlotGroup;
			this.SlotFlags = slotDefinition.SlotFlags;
		}

		// Token: 0x04000AE7 RID: 2791
		public readonly EntityUid Equipee;

		// Token: 0x04000AE8 RID: 2792
		public readonly EntityUid Equipment;

		// Token: 0x04000AE9 RID: 2793
		public readonly string Slot;

		// Token: 0x04000AEA RID: 2794
		public readonly string SlotGroup;

		// Token: 0x04000AEB RID: 2795
		public readonly SlotFlags SlotFlags;
	}
}
