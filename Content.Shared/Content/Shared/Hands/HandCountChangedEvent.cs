using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200043A RID: 1082
	public sealed class HandCountChangedEvent : EntityEventArgs
	{
		// Token: 0x06000CED RID: 3309 RVA: 0x0002A5E6 File Offset: 0x000287E6
		public HandCountChangedEvent(EntityUid sender)
		{
			this.Sender = sender;
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000CEE RID: 3310 RVA: 0x0002A5F5 File Offset: 0x000287F5
		public EntityUid Sender { get; }
	}
}
