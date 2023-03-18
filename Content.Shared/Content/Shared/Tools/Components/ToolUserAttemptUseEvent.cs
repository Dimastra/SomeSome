using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components
{
	// Token: 0x020000BE RID: 190
	[ByRefEvent]
	public struct ToolUserAttemptUseEvent
	{
		// Token: 0x06000219 RID: 537 RVA: 0x0000AA6C File Offset: 0x00008C6C
		public ToolUserAttemptUseEvent(EntityUid user, EntityUid? target)
		{
			this.Cancelled = false;
			this.User = user;
			this.Target = target;
		}

		// Token: 0x0400028A RID: 650
		public EntityUid User;

		// Token: 0x0400028B RID: 651
		public EntityUid? Target;

		// Token: 0x0400028C RID: 652
		public bool Cancelled;
	}
}
