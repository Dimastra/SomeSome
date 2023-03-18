using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000433 RID: 1075
	public sealed class DidEquipHandEvent : EquippedHandEvent
	{
		// Token: 0x06000CE2 RID: 3298 RVA: 0x0002A56C File Offset: 0x0002876C
		[NullableContext(1)]
		public DidEquipHandEvent(EntityUid user, EntityUid unequipped, Hand hand) : base(user, unequipped, hand)
		{
		}
	}
}
