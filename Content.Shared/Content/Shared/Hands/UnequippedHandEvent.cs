using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000430 RID: 1072
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class UnequippedHandEvent : HandledEntityEventArgs
	{
		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000CDC RID: 3292 RVA: 0x0002A521 File Offset: 0x00028721
		public EntityUid User { get; }

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000CDD RID: 3293 RVA: 0x0002A529 File Offset: 0x00028729
		public EntityUid Unequipped { get; }

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x0002A531 File Offset: 0x00028731
		public Hand Hand { get; }

		// Token: 0x06000CDF RID: 3295 RVA: 0x0002A539 File Offset: 0x00028739
		public UnequippedHandEvent(EntityUid user, EntityUid unequipped, Hand hand)
		{
			this.User = user;
			this.Unequipped = unequipped;
			this.Hand = hand;
		}
	}
}
