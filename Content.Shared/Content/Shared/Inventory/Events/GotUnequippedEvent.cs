using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BF RID: 959
	public sealed class GotUnequippedEvent : UnequippedEventBase
	{
		// Token: 0x06000B05 RID: 2821 RVA: 0x00024549 File Offset: 0x00022749
		[NullableContext(1)]
		public GotUnequippedEvent(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipment, slotDefinition)
		{
		}
	}
}
