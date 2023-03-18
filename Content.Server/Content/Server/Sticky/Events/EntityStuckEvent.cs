using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Sticky.Events
{
	// Token: 0x02000172 RID: 370
	public sealed class EntityStuckEvent : EntityEventArgs
	{
		// Token: 0x0600075C RID: 1884 RVA: 0x000249A7 File Offset: 0x00022BA7
		public EntityStuckEvent(EntityUid target, EntityUid user)
		{
			this.Target = target;
			this.User = user;
		}

		// Token: 0x04000465 RID: 1125
		public readonly EntityUid Target;

		// Token: 0x04000466 RID: 1126
		public readonly EntityUid User;
	}
}
