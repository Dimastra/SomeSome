using System;
using Robust.Shared.GameObjects;

namespace Content.Server.UserInterface
{
	// Token: 0x020000F5 RID: 245
	public sealed class UserOpenActivatableUIAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00015EA6 File Offset: 0x000140A6
		public EntityUid User { get; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00015EAE File Offset: 0x000140AE
		public EntityUid Target { get; }

		// Token: 0x0600048B RID: 1163 RVA: 0x00015EB6 File Offset: 0x000140B6
		public UserOpenActivatableUIAttemptEvent(EntityUid who, EntityUid target)
		{
			this.User = who;
			this.Target = target;
		}
	}
}
