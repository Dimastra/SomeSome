using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Configurations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BB RID: 1211
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MaxTimeRestartRuleSystem : GameRuleSystem
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x00080044 File Offset: 0x0007E244
		public override string Prototype
		{
			get
			{
				return "MaxTimeRestart";
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x060018B7 RID: 6327 RVA: 0x0008004B File Offset: 0x0007E24B
		// (set) Token: 0x060018B8 RID: 6328 RVA: 0x00080053 File Offset: 0x0007E253
		public TimeSpan RoundMaxTime { get; set; } = TimeSpan.FromMinutes(5.0);

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x060018B9 RID: 6329 RVA: 0x0008005C File Offset: 0x0007E25C
		// (set) Token: 0x060018BA RID: 6330 RVA: 0x00080064 File Offset: 0x0007E264
		public TimeSpan RoundEndDelay { get; set; } = TimeSpan.FromSeconds(10.0);

		// Token: 0x060018BB RID: 6331 RVA: 0x0008006D File Offset: 0x0007E26D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.RunLevelChanged), null, null);
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0008008C File Offset: 0x0007E28C
		public override void Started()
		{
			MaxTimeRestartRuleConfiguration maxTimeRestartConfig = this.Configuration as MaxTimeRestartRuleConfiguration;
			if (maxTimeRestartConfig == null)
			{
				return;
			}
			this.RoundMaxTime = maxTimeRestartConfig.RoundMaxTime;
			this.RoundEndDelay = maxTimeRestartConfig.RoundEndDelay;
			if (this.GameTicker.RunLevel == GameRunLevel.InRound)
			{
				this.RestartTimer();
			}
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x000800D5 File Offset: 0x0007E2D5
		public override void Ended()
		{
			this.StopTimer();
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x000800DD File Offset: 0x0007E2DD
		public void RestartTimer()
		{
			this._timerCancel.Cancel();
			this._timerCancel = new CancellationTokenSource();
			Timer.Spawn(this.RoundMaxTime, new Action(this.TimerFired), this._timerCancel.Token);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x00080117 File Offset: 0x0007E317
		public void StopTimer()
		{
			this._timerCancel.Cancel();
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x00080124 File Offset: 0x0007E324
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

		// Token: 0x060018C1 RID: 6337 RVA: 0x000801B0 File Offset: 0x0007E3B0
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

		// Token: 0x04000F5E RID: 3934
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F5F RID: 3935
		private CancellationTokenSource _timerCancel = new CancellationTokenSource();
	}
}
