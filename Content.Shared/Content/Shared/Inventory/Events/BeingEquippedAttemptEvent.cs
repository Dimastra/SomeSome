using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B3 RID: 947
	public sealed class BeingEquippedAttemptEvent : EquipAttemptBase
	{
		// Token: 0x06000AF9 RID: 2809 RVA: 0x00024426 File Offset: 0x00022626
		[NullableContext(1)]
		public BeingEquippedAttemptEvent(EntityUid equipee, EntityUid equipTarget, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipTarget, equipment, slotDefinition)
		{
		}
	}
}
