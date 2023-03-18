using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory.Events
{
	// Token: 0x020003B6 RID: 950
	public sealed class DidEquipEvent : EquippedEventBase
	{
		// Token: 0x06000AFC RID: 2812 RVA: 0x0002447A File Offset: 0x0002267A
		[NullableContext(1)]
		public DidEquipEvent(EntityUid equipee, EntityUid equipment, SlotDefinition slotDefinition) : base(equipee, equipment, slotDefinition)
		{
		}
	}
}
