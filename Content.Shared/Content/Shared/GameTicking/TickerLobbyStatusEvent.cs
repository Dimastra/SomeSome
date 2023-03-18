using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000467 RID: 1127
	[NullableContext(2)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class TickerLobbyStatusEvent : EntityEventArgs
	{
		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000DA6 RID: 3494 RVA: 0x0002CA1F File Offset: 0x0002AC1F
		public bool IsRoundStarted { get; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x0002CA27 File Offset: 0x0002AC27
		public string LobbySong { get; }

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x0002CA2F File Offset: 0x0002AC2F
		public string LobbyBackground { get; }

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000DA9 RID: 3497 RVA: 0x0002CA37 File Offset: 0x0002AC37
		public bool YouAreReady { get; }

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0002CA3F File Offset: 0x0002AC3F
		public TimeSpan StartTime { get; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x0002CA47 File Offset: 0x0002AC47
		public TimeSpan RoundStartTimeSpan { get; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x0002CA4F File Offset: 0x0002AC4F
		public bool Paused { get; }

		// Token: 0x06000DAD RID: 3501 RVA: 0x0002CA57 File Offset: 0x0002AC57
		public TickerLobbyStatusEvent(bool isRoundStarted, string lobbySong, string lobbyBackground, bool youAreReady, TimeSpan startTime, TimeSpan roundStartTimeSpan, bool paused)
		{
			this.IsRoundStarted = isRoundStarted;
			this.LobbySong = lobbySong;
			this.LobbyBackground = lobbyBackground;
			this.YouAreReady = youAreReady;
			this.StartTime = startTime;
			this.RoundStartTimeSpan = roundStartTimeSpan;
			this.Paused = paused;
		}
	}
}
