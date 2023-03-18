using System;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200042F RID: 1071
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class EquippedHandEvent : HandledEntityEventArgs
	{
		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000CD8 RID: 3288 RVA: 0x0002A4EC File Offset: 0x000286EC
		public EntityUid User { get; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000CD9 RID: 3289 RVA: 0x0002A4F4 File Offset: 0x000286F4
		public EntityUid Equipped { get; }

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000CDA RID: 3290 RVA: 0x0002A4FC File Offset: 0x000286FC
		public Hand Hand { get; }

		// Token: 0x06000CDB RID: 3291 RVA: 0x0002A504 File Offset: 0x00028704
		public EquippedHandEvent(EntityUid user, EntityUid equipped, Hand hand)
		{
			this.User = user;
			this.Equipped = equipped;
			this.Hand = hand;
		}
	}
}
