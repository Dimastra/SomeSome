using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003BE RID: 958
	public sealed class DidUnequipEvent : UnequippedEventBase
	{
		// Token: 0x06000B04 RID: 2820 RVA: 0x0002453E File Offset: 0x0002273E
		[NullableContext(1)]
		public DidUnequipEvent(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipment, slotDefinition)
		{
		}
	}
}
