using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking
{
	// Token: 0x0200046C RID: 1132
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RoundEndMessageEvent : EntityEventArgs
	{
		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000DB8 RID: 3512 RVA: 0x0002CB0E File Offset: 0x0002AD0E
		public string GamemodeTitle { get; }

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000DB9 RID: 3513 RVA: 0x0002CB16 File Offset: 0x0002AD16
		public string RoundEndText { get; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000DBA RID: 3514 RVA: 0x0002CB1E File Offset: 0x0002AD1E
		public TimeSpan RoundDuration { get; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000DBB RID: 3515 RVA: 0x0002CB26 File Offset: 0x0002AD26
		public int RoundId { get; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000DBC RID: 3516 RVA: 0x0002CB2E File Offset: 0x0002AD2E
		public int PlayerCount { get; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06000DBD RID: 3517 RVA: 0x0002CB36 File Offset: 0x0002AD36
		public RoundEndMessageEvent.RoundEndPlayerInfo[] AllPlayersEndInfo { get; }

		// Token: 0x06000DBE RID: 3518 RVA: 0x0002CB40 File Offset: 0x0002AD40
		public RoundEndMessageEvent(string gamemodeTitle, string roundEndText, TimeSpan roundDuration, int roundId, int playerCount, RoundEndMessageEvent.RoundEndPlayerInfo[] allPlayersEndInfo, [Nullable(2)] string lobbySong, [Nullable(2)] string restartSound)
		{
			this.GamemodeTitle = gamemodeTitle;
			this.RoundEndText = roundEndText;
			this.RoundDuration = roundDuration;
			this.RoundId = roundId;
			this.PlayerCount = playerCount;
			this.AllPlayersEndInfo = allPlayersEndInfo;
			this.LobbySong = lobbySong;
			this.RestartSound = restartSound;
		}

		// Token: 0x04000D10 RID: 3344
		[Nullable(2)]
		public string LobbySong;

		// Token: 0x04000D11 RID: 3345
		[Nullable(2)]
		public string RestartSound;

		// Token: 0x02000808 RID: 2056
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public struct RoundEndPlayerInfo
		{
			// Token: 0x040018AC RID: 6316
			public string PlayerOOCName;

			// Token: 0x040018AD RID: 6317
			[Nullable(2)]
			public string PlayerICName;

			// Token: 0x040018AE RID: 6318
			public string Role;

			// Token: 0x040018AF RID: 6319
			public EntityUid? PlayerEntityUid;

			// Token: 0x040018B0 RID: 6320
			public bool Antag;

			// Token: 0x040018B1 RID: 6321
			public bool Observer;

			// Token: 0x040018B2 RID: 6322
			public bool Connected;
		}
	}
}
