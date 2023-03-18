using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Mech.Components
{
	// Token: 0x020003CB RID: 971
	public sealed class MechEntryFinishedEvent : EntityEventArgs
	{
		// Token: 0x060013FD RID: 5117 RVA: 0x0006815B File Offset: 0x0006635B
		public MechEntryFinishedEvent(EntityUid user)
		{
			this.User = user;
		}

		// Token: 0x04000C67 RID: 3175
		public EntityUid User;
	}
}
