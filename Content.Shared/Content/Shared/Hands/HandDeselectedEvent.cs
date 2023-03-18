using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200042A RID: 1066
	public sealed class HandDeselectedEvent : HandledEntityEventArgs
	{
		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000CCD RID: 3277 RVA: 0x0002A45C File Offset: 0x0002865C
		public EntityUid User { get; }

		// Token: 0x06000CCE RID: 3278 RVA: 0x0002A464 File Offset: 0x00028664
		public HandDeselectedEvent(EntityUid user)
		{
			this.User = user;
		}
	}
}
