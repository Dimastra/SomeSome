using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000668 RID: 1640
	public sealed class ShiverAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x0600141B RID: 5147 RVA: 0x00043423 File Offset: 0x00041623
		public ShiverAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x0600141C RID: 5148 RVA: 0x00043432 File Offset: 0x00041632
		public EntityUid Uid { get; }
	}
}
