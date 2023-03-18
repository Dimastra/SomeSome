using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Pulling.Events
{
	// Token: 0x0200023A RID: 570
	public sealed class BeingPulledAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000666 RID: 1638 RVA: 0x00017055 File Offset: 0x00015255
		public BeingPulledAttemptEvent(EntityUid puller, EntityUid pulled)
		{
			this.Puller = puller;
			this.Pulled = pulled;
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x0001706B File Offset: 0x0001526B
		public EntityUid Puller { get; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000668 RID: 1640 RVA: 0x00017073 File Offset: 0x00015273
		public EntityUid Pulled { get; }
	}
}
