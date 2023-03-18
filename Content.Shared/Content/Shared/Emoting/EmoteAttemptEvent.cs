using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Emoting
{
	// Token: 0x020004C3 RID: 1219
	public sealed class EmoteAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000EB4 RID: 3764 RVA: 0x0002F4F3 File Offset: 0x0002D6F3
		public EmoteAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0002F502 File Offset: 0x0002D702
		public EntityUid Uid { get; }
	}
}
