using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000431 RID: 1073
	public sealed class GotEquippedHandEvent : EquippedHandEvent
	{
		// Token: 0x06000CE0 RID: 3296 RVA: 0x0002A556 File Offset: 0x00028756
		[NullableContext(1)]
		public GotEquippedHandEvent(EntityUid user, EntityUid unequipped, Hand hand) : base(user, unequipped, hand)
		{
		}
	}
}
