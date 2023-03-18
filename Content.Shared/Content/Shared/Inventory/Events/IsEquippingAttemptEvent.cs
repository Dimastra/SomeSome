using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B4 RID: 948
	public sealed class IsEquippingAttemptEvent : EquipAttemptBase
	{
		// Token: 0x06000AFA RID: 2810 RVA: 0x00024433 File Offset: 0x00022633
		[NullableContext(1)]
		public IsEquippingAttemptEvent(EntityUid equipee, EntityUid equipTarget, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipTarget, equipment, slotDefinition)
		{
		}
	}
}
