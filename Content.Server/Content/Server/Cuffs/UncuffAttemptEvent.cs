using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Cuffs
{
	// Token: 0x020005D4 RID: 1492
	public sealed class UncuffAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06001FC4 RID: 8132 RVA: 0x000A6698 File Offset: 0x000A4898
		public UncuffAttemptEvent(EntityUid user, EntityUid target)
		{
			this.User = user;
			this.Target = target;
		}

		// Token: 0x040013B9 RID: 5049
		public readonly EntityUid User;

		// Token: 0x040013BA RID: 5050
		public readonly EntityUid Target;
	}
}
