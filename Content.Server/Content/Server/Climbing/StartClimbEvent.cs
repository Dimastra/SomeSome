using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Climbing
{
	// Token: 0x02000648 RID: 1608
	public sealed class StartClimbEvent : EntityEventArgs
	{
		// Token: 0x0600222A RID: 8746 RVA: 0x000B2E0E File Offset: 0x000B100E
		public StartClimbEvent(EntityUid climbable)
		{
			this.Climbable = climbable;
		}

		// Token: 0x04001515 RID: 5397
		public EntityUid Climbable;
	}
}
