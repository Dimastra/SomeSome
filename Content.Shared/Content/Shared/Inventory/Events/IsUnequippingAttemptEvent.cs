using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BC RID: 956
	public sealed class IsUnequippingAttemptEvent : UnequipAttemptEventBase
	{
		// Token: 0x06000B02 RID: 2818 RVA: 0x00024503 File Offset: 0x00022703
		[NullableContext(1)]
		public IsUnequippingAttemptEvent(EntityUid unequipee, EntityUid unEquipTarget, EntityUid equipment, SlotDefinition slotDefinition) : base(unequipee, unEquipTarget, equipment, slotDefinition)
		{
		}
	}
}
