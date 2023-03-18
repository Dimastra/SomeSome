using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Sticky.Events
{
	// Token: 0x02000173 RID: 371
	public sealed class EntityUnstuckEvent : EntityEventArgs
	{
		// Token: 0x0600075D RID: 1885 RVA: 0x000249BD File Offset: 0x00022BBD
		public EntityUnstuckEvent(EntityUid target, EntityUid user)
		{
			this.Target = target;
			this.User = user;
		}

		// Token: 0x04000467 RID: 1127
		public readonly EntityUid Target;

		// Token: 0x04000468 RID: 1128
		public readonly EntityUid User;
	}
}
