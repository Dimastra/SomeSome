using System;
using Robust.Shared.GameObjects;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F4 RID: 244
	public sealed class ActivatableUIOpenAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x00015E8F File Offset: 0x0001408F
		public EntityUid User { get; }

		// Token: 0x06000488 RID: 1160 RVA: 0x00015E97 File Offset: 0x00014097
		public ActivatableUIOpenAttemptEvent(EntityUid who)
		{
			this.User = who;
		}
	}
}
