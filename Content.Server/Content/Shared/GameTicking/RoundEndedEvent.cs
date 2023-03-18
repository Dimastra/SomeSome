using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000012 RID: 18
	public sealed class RoundEndedEvent : EntityEventArgs
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002BD4 File Offset: 0x00000DD4
		public int RoundId { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002BDC File Offset: 0x00000DDC
		public TimeSpan RoundDuration { get; }

		// Token: 0x06000036 RID: 54 RVA: 0x00002BE4 File Offset: 0x00000DE4
		public RoundEndedEvent(int roundId, TimeSpan roundDuration)
		{
			this.RoundId = roundId;
			this.RoundDuration = roundDuration;
		}
	}
}
