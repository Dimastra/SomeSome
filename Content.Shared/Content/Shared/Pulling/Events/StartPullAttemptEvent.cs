using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Pulling.Events
{
	// Token: 0x0200023B RID: 571
	public sealed class StartPullAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000669 RID: 1641 RVA: 0x0001707B File Offset: 0x0001527B
		public StartPullAttemptEvent(EntityUid puller, EntityUid pulled)
		{
			this.Puller = puller;
			this.Pulled = pulled;
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x00017091 File Offset: 0x00015291
		public EntityUid Puller { get; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600066B RID: 1643 RVA: 0x00017099 File Offset: 0x00015299
		public EntityUid Pulled { get; }
	}
}
