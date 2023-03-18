using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000469 RID: 1129
	[NetSerializable]
	[Serializable]
	public sealed class TickerLobbyCountdownEvent : EntityEventArgs
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x0002CAAB File Offset: 0x0002ACAB
		public TimeSpan StartTime { get; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x0002CAB3 File Offset: 0x0002ACB3
		public bool Paused { get; }

		// Token: 0x06000DB2 RID: 3506 RVA: 0x0002CABB File Offset: 0x0002ACBB
		public TickerLobbyCountdownEvent(TimeSpan startTime, bool paused)
		{
			this.StartTime = startTime;
			this.Paused = paused;
		}
	}
}
