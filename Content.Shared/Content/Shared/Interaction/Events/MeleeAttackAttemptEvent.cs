using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D9 RID: 985
	[ByRefEvent]
	public struct MeleeAttackAttemptEvent
	{
		// Token: 0x06000B90 RID: 2960 RVA: 0x0002634F File Offset: 0x0002454F
		public MeleeAttackAttemptEvent(EntityUid user)
		{
			this.Cancelled = false;
			this.User = user;
		}

		// Token: 0x04000B3E RID: 2878
		public bool Cancelled;

		// Token: 0x04000B3F RID: 2879
		public readonly EntityUid User;
	}
}
