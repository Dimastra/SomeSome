using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000434 RID: 1076
	public sealed class DidUnequipHandEvent : UnequippedHandEvent
	{
		// Token: 0x06000CE3 RID: 3299 RVA: 0x0002A577 File Offset: 0x00028777
		[NullableContext(1)]
		public DidUnequipHandEvent(EntityUid user, EntityUid unequipped, Hand hand) : base(user, unequipped, hand)
		{
		}
	}
}
