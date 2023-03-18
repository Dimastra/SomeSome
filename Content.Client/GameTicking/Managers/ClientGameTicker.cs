using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Audio;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.RoundEnd;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.GameWindow;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.ViewVariables;

namespace Content.Client.GameTicking.Managers
{
	// Token: 0x02000306 RID: 774
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class ClientGameTicker : SharedGameTicker
	{
		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x0600136B RID: 4971 RVA: 0x00073245 File Offset: 0x00071445
		// (set) Token: 0x0600136C RID: 4972 RVA: 0x0007324D File Offset: 0x0007144D
		[ViewVariables]
		public bool AreWeReady { get; private set; }

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x00073256 File Offset: 0x00071456
		// (set) Token: 0x0600136E RID: 4974 RVA: 0x0007325E File Offset: 0x0007145E
		[ViewVariables]
		public bool IsGameStarted { get; private set; }

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x00073267 File Offset: 0x00071467
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x0007326F File Offset: 0x0007146F
		[ViewVariables]
		public string LobbySong { get; private set; }

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x00073278 File Offset: 0x00071478
		// (set) Token: 0x06001372 RID: 4978 RVA: 0x00073280 File Offset: 0x00071480
		[ViewVariables]
		public string RestartSound { get; private set; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x00073289 File Offset: 0x00071489
		// (set) Token: 0x06001374 RID: 4980 RVA: 0x00073291 File Offset: 0x00071491
		[ViewVariables]
		public string LobbyBackground { get; private set; }

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06001375 RID: 4981 RVA: 0x0007329A File Offset: 0x0007149A
		// (set) Token: 0x06001376 RID: 4982 RVA: 0x000732A2 File Offset: 0x000714A2
		[ViewVariables]
		public bool DisallowedLateJoin { get; private set; }

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06001377 RID: 4983 RVA: 0x000732AB File Offset: 0x000714AB
		// (set) Token: 0x06001378 RID: 4984 RVA: 0x000732B3 File Offset: 0x000714B3
		[ViewVariables]
		public string ServerInfoBlob { get; private set; }

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x000732BC File Offset: 0x000714BC
		// (set) Token: 0x0600137A RID: 4986 RVA: 0x000732C4 File Offset: 0x000714C4
		[ViewVariables]
		public TimeSpan StartTime { get; private set; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x000732CD File Offset: 0x000714CD
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x000732D5 File Offset: 0x000714D5
		[ViewVariables]
		public TimeSpan RoundStartTimeSpan { get; private set; }

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x000732DE File Offset: 0x000714DE
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x000732E6 File Offset: 0x000714E6
		[ViewVariables]
		public bool Paused { get; private set; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x000732EF File Offset: 0x000714EF
		[Nullable(1)]
		[ViewVariables]
		public IReadOnlyDictionary<EntityUid, Dictionary<string, uint?>> JobsAvailable
		{
			[NullableContext(1)]
			get
			{
				return this._jobsAvailable;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x000732F7 File Offset: 0x000714F7
		[Nullable(1)]
		[ViewVariables]
		public IReadOnlyDictionary<EntityUid, string> StationNames
		{
			[NullableContext(1)]
			get
			{
				return this._stationNames;
			}
		}

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06001381 RID: 4993 RVA: 0x00073300 File Offset: 0x00071500
		// (remove) Token: 0x06001382 RID: 4994 RVA: 0x00073338 File Offset: 0x00071538
		public event Action InfoBlobUpdated;

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06001383 RID: 4995 RVA: 0x00073370 File Offset: 0x00071570
		// (remove) Token: 0x06001384 RID: 4996 RVA: 0x000733A8 File Offset: 0x000715A8
		public event Action LobbyStatusUpdated;

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06001385 RID: 4997 RVA: 0x000733E0 File Offset: 0x000715E0
		// (remove) Token: 0x06001386 RID: 4998 RVA: 0x00073418 File Offset: 0x00071618
		public event Action LobbyReadyUpdated;

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06001387 RID: 4999 RVA: 0x00073450 File Offset: 0x00071650
		// (remove) Token: 0x06001388 RID: 5000 RVA: 0x00073488 File Offset: 0x00071688
		public event Action LobbyLateJoinStatusUpdated;

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x06001389 RID: 5001 RVA: 0x000734C0 File Offset: 0x000716C0
		// (remove) Token: 0x0600138A RID: 5002 RVA: 0x000734F8 File Offset: 0x000716F8
		[Nullable(new byte[]
		{
			2,
			1,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1,
			1
		})]
		public event Action<IReadOnlyDictionary<EntityUid, Dictionary<string, uint?>>> LobbyJobsAvailableUpdated;

		// Token: 0x0600138B RID: 5003 RVA: 0x00073530 File Offset: 0x00071730
		public override void Initialize()
		{
			base.SubscribeNetworkEvent<TickerJoinLobbyEvent>(new EntityEventHandler<TickerJoinLobbyEvent>(this.JoinLobby), null, null);
			base.SubscribeNetworkEvent<TickerJoinGameEvent>(new EntityEventHandler<TickerJoinGameEvent>(this.JoinGame), null, null);
			base.SubscribeNetworkEvent<TickerLobbyStatusEvent>(new EntityEventHandler<TickerLobbyStatusEvent>(this.LobbyStatus), null, null);
			base.SubscribeNetworkEvent<TickerLobbyInfoEvent>(new EntityEventHandler<TickerLobbyInfoEvent>(this.LobbyInfo), null, null);
			base.SubscribeNetworkEvent<TickerLobbyCountdownEvent>(new EntityEventHandler<TickerLobbyCountdownEvent>(this.LobbyCountdown), null, null);
			base.SubscribeNetworkEvent<TickerLobbyReadyEvent>(new EntityEventHandler<TickerLobbyReadyEvent>(this.LobbyReady), null, null);
			base.SubscribeNetworkEvent<RoundEndMessageEvent>(new EntityEventHandler<RoundEndMessageEvent>(this.RoundEnd), null, null);
			base.SubscribeNetworkEvent<RequestWindowAttentionEvent>(delegate(RequestWindowAttentionEvent msg)
			{
				IoCManager.Resolve<IClyde>().RequestWindowAttention();
			}, null, null);
			base.SubscribeNetworkEvent<TickerLateJoinStatusEvent>(new EntityEventHandler<TickerLateJoinStatusEvent>(this.LateJoinStatus), null, null);
			base.SubscribeNetworkEvent<TickerJobsAvailableEvent>(new EntityEventHandler<TickerJobsAvailableEvent>(this.UpdateJobsAvailable), null, null);
			base.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.RoundRestartCleanup), null, null);
			this._initialized = true;
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x00073633 File Offset: 0x00071833
		[NullableContext(1)]
		private void LateJoinStatus(TickerLateJoinStatusEvent message)
		{
			this.DisallowedLateJoin = message.Disallowed;
			Action lobbyLateJoinStatusUpdated = this.LobbyLateJoinStatusUpdated;
			if (lobbyLateJoinStatusUpdated == null)
			{
				return;
			}
			lobbyLateJoinStatusUpdated();
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x00073651 File Offset: 0x00071851
		[NullableContext(1)]
		private void UpdateJobsAvailable(TickerJobsAvailableEvent message)
		{
			this._jobsAvailable = message.JobsAvailableByStation;
			this._stationNames = message.StationNames;
			Action<IReadOnlyDictionary<EntityUid, Dictionary<string, uint?>>> lobbyJobsAvailableUpdated = this.LobbyJobsAvailableUpdated;
			if (lobbyJobsAvailableUpdated == null)
			{
				return;
			}
			lobbyJobsAvailableUpdated(this.JobsAvailable);
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x00073681 File Offset: 0x00071881
		[NullableContext(1)]
		private void JoinLobby(TickerJoinLobbyEvent message)
		{
			this._stateManager.RequestStateChange<LobbyState>();
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x00073690 File Offset: 0x00071890
		[NullableContext(1)]
		private void LobbyStatus(TickerLobbyStatusEvent message)
		{
			this.StartTime = message.StartTime;
			this.RoundStartTimeSpan = message.RoundStartTimeSpan;
			this.IsGameStarted = message.IsRoundStarted;
			this.AreWeReady = message.YouAreReady;
			this.LobbySong = message.LobbySong;
			this.LobbyBackground = message.LobbyBackground;
			this.Paused = message.Paused;
			Action lobbyStatusUpdated = this.LobbyStatusUpdated;
			if (lobbyStatusUpdated == null)
			{
				return;
			}
			lobbyStatusUpdated();
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x00073701 File Offset: 0x00071901
		[NullableContext(1)]
		private void LobbyInfo(TickerLobbyInfoEvent message)
		{
			this.ServerInfoBlob = message.TextBlob;
			Action infoBlobUpdated = this.InfoBlobUpdated;
			if (infoBlobUpdated == null)
			{
				return;
			}
			infoBlobUpdated();
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x0007371F File Offset: 0x0007191F
		[NullableContext(1)]
		private void JoinGame(TickerJoinGameEvent message)
		{
			this._stateManager.RequestStateChange<GameplayState>();
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x0007372D File Offset: 0x0007192D
		[NullableContext(1)]
		private void LobbyCountdown(TickerLobbyCountdownEvent message)
		{
			this.StartTime = message.StartTime;
			this.Paused = message.Paused;
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00073747 File Offset: 0x00071947
		[NullableContext(1)]
		private void LobbyReady(TickerLobbyReadyEvent message)
		{
			Action lobbyReadyUpdated = this.LobbyReadyUpdated;
			if (lobbyReadyUpdated == null)
			{
				return;
			}
			lobbyReadyUpdated();
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x0007375C File Offset: 0x0007195C
		[NullableContext(1)]
		private void RoundEnd(RoundEndMessageEvent message)
		{
			if (message.LobbySong != null)
			{
				this.LobbySong = message.LobbySong;
				EntitySystem.Get<MusicAudioSystem>().StartLobbyMusic();
			}
			this.RestartSound = message.RestartSound;
			new RoundEndSummaryWindow(message.GamemodeTitle, message.RoundEndText, message.RoundDuration, message.RoundId, message.AllPlayersEndInfo, this._entityManager);
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x000737C0 File Offset: 0x000719C0
		[NullableContext(1)]
		private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
		{
			if (string.IsNullOrEmpty(this.RestartSound))
			{
				return;
			}
			if (!this._configManager.GetCVar<bool>(CCVars.RestartSoundsEnabled))
			{
				this.RestartSound = null;
				return;
			}
			SoundSystem.Play(this.RestartSound, Filter.Empty(), null);
			this.RestartSound = null;
		}

		// Token: 0x040009B3 RID: 2483
		[Nullable(1)]
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x040009B4 RID: 2484
		[Nullable(1)]
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x040009B5 RID: 2485
		[Nullable(1)]
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x040009B6 RID: 2486
		[ViewVariables]
		private bool _initialized;

		// Token: 0x040009B7 RID: 2487
		[Nullable(1)]
		private Dictionary<EntityUid, Dictionary<string, uint?>> _jobsAvailable = new Dictionary<EntityUid, Dictionary<string, uint?>>();

		// Token: 0x040009B8 RID: 2488
		[Nullable(1)]
		private Dictionary<EntityUid, string> _stationNames = new Dictionary<EntityUid, string>();
	}
}
