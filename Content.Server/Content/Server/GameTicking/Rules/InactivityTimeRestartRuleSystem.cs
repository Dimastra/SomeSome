using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Configurations;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BA RID: 1210
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InactivityTimeRestartRuleSystem : GameRuleSystem
	{
		// Token: 0x17000380 RID: 896
		// (get) Token: 0x060018A7 RID: 6311 RVA: 0x0007FE0C File Offset: 0x0007E00C
		public override string Prototype
		{
			get
			{
				return "InactivityTimeRestart";
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x060018A8 RID: 6312 RVA: 0x0007FE13 File Offset: 0x0007E013
		// (set) Token: 0x060018A9 RID: 6313 RVA: 0x0007FE1B File Offset: 0x0007E01B
		public TimeSpan InactivityMaxTime { get; set; } = TimeSpan.FromMinutes(10.0);

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x060018AA RID: 6314 RVA: 0x0007FE24 File Offset: 0x0007E024
		// (set) Token: 0x060018AB RID: 6315 RVA: 0x0007FE2C File Offset: 0x0007E02C
		public TimeSpan RoundEndDelay { get; set; } = TimeSpan.FromSeconds(10.0);

		// Token: 0x060018AC RID: 6316 RVA: 0x0007FE35 File Offset: 0x0007E035
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.RunLevelChanged), null, null);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x0007FE54 File Offset: 0x0007E054
		public override void Started()
		{
			InactivityGameRuleConfiguration inactivityConfig = this.Configuration as InactivityGameRuleConfiguration;
			if (inactivityConfig == null)
			{
				return;
			}
			this.InactivityMaxTime = inactivityConfig.InactivityMaxTime;
			this.RoundEndDelay = inactivityConfig.RoundEndDelay;
			this._playerManager.PlayerStatusChanged += this.PlayerStatusChanged;
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x0007FEA0 File Offset: 0x0007E0A0
		public override void Ended()
		{
			this._playerManager.PlayerStatusChanged -= this.PlayerStatusChanged;
			this.StopTimer();
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x0007FEBF File Offset: 0x0007E0BF
		public void RestartTimer()
		{
			this._timerCancel.Cancel();
			this._timerCancel = new CancellationTokenSource();
			Timer.Spawn(this.InactivityMaxTime, new Action(this.TimerFired), this._timerCancel.Token);
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x0007FEF9 File Offset: 0x0007E0F9
		public void StopTimer()
		{
			this._timerCancel.Cancel();
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x0007FF08 File Offset: 0x0007E108
		private void TimerFired()
		{
			this.GameTicker.EndRound(Loc.GetString("rule-time-has-run-out"));
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-restarting-in-seconds", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seconds", (int)this.RoundEndDelay.TotalSeconds)
			}), null);
			Timer.Spawn(this.RoundEndDelay, delegate()
			{
				this.GameTicker.RestartRound();
			}, default(CancellationToken));
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x0007FF94 File Offset: 0x0007E194
		private void RunLevelChanged(GameRunLevelChangedEvent args)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			switch (args.New)
			{
			case GameRunLevel.PreRoundLobby:
			case GameRunLevel.PostRound:
				this.StopTimer();
				return;
			case GameRunLevel.InRound:
				this.RestartTimer();
				return;
			default:
				return;
			}
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x0007FFD1 File Offset: 0x0007E1D1
		private void PlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (this.GameTicker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			if (this._playerManager.PlayerCount == 0)
			{
				this.RestartTimer();
				return;
			}
			this.StopTimer();
		}

		// Token: 0x04000F59 RID: 3929
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F5A RID: 3930
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000F5B RID: 3931
		private CancellationTokenSource _timerCancel = new CancellationTokenSource();
	}
}
