using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200042B RID: 1067
	public sealed class HandSelectedEvent : HandledEntityEventArgs
	{
		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x0002A473 File Offset: 0x00028673
		public EntityUid User { get; }

		// Token: 0x06000CD0 RID: 3280 RVA: 0x0002A47B File Offset: 0x0002867B
		public HandSelectedEvent(EntityUid user)
		{
			this.User = user;
		}
	}
}
