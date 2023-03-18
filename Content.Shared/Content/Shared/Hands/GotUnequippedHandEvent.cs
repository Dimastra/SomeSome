using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000432 RID: 1074
	public sealed class GotUnequippedHandEvent : UnequippedHandEvent
	{
		// Token: 0x06000CE1 RID: 3297 RVA: 0x0002A561 File Offset: 0x00028761
		[NullableContext(1)]
		public GotUnequippedHandEvent(EntityUid user, EntityUid unequipped, Hand hand) : base(user, unequipped, hand)
		{
		}
	}
}
