using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Speech
{
	// Token: 0x0200017A RID: 378
	public sealed class SpeakAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000489 RID: 1161 RVA: 0x00011E95 File Offset: 0x00010095
		public SpeakAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x00011EA4 File Offset: 0x000100A4
		public EntityUid Uid { get; }
	}
}
