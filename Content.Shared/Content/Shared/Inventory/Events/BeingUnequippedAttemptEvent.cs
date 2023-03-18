using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BB RID: 955
	public sealed class BeingUnequippedAttemptEvent : UnequipAttemptEventBase
	{
		// Token: 0x06000B01 RID: 2817 RVA: 0x000244F6 File Offset: 0x000226F6
		[NullableContext(1)]
		public BeingUnequippedAttemptEvent(EntityUid unequipee, EntityUid unEquipTarget, EntityUid equipment, SlotDefinition slotDefinition) : base(unequipee, unEquipTarget, equipment, slotDefinition)
		{
		}
	}
}
