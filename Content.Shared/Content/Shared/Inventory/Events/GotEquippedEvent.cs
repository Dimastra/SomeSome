using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B7 RID: 951
	public sealed class GotEquippedEvent : EquippedEventBase
	{
		// Token: 0x06000AFD RID: 2813 RVA: 0x00024485 File Offset: 0x00022685
		[NullableContext(1)]
		public GotEquippedEvent(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipment, slotDefinition)
		{
		}
	}
}
