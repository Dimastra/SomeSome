using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Flash
{
	// Token: 0x020004FB RID: 1275
	public sealed class FlashAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06001A3E RID: 6718 RVA: 0x0008AA3D File Offset: 0x00088C3D
		public FlashAttemptEvent(EntityUid target, EntityUid? user, EntityUid? used, bool massFlash)
		{
			this.Target = target;
			this.User = user;
			this.Used = used;
			this.MassFlash = massFlash;
		}

		// Token: 0x040010A0 RID: 4256
		public readonly EntityUid Target;

		// Token: 0x040010A1 RID: 4257
		public readonly EntityUid? User;

		// Token: 0x040010A2 RID: 4258
		public readonly EntityUid? Used;

		// Token: 0x040010A3 RID: 4259
		public readonly bool MassFlash;
	}
}
