using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Access.Systems;
using Content.Server.Administration.Logs;
using Content.Server.AlertLevel;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Systems;
using Content.Server.UtkaIntegration;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.RoundEnd
{
	// Token: 0x02000224 RID: 548
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundEndSystem : EntitySystem
	{
		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x00039380 File Offset: 0x00037580
		// (set) Token: 0x06000AE2 RID: 2786 RVA: 0x00039388 File Offset: 0x00037588
		public TimeSpan DefaultCooldownDuration { get; set; } = TimeSpan.FromSeconds(30.0);

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x00039391 File Offset: 0x00037591
		// (set) Token: 0x06000AE4 RID: 2788 RVA: 0x00039399 File Offset: 0x00037599
		public TimeSpan DefaultCountdownDuration { get; set; } = TimeSpan.FromMinutes(10.0);

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x000393A2 File Offset: 0x000375A2
		// (set) Token: 0x06000AE6 RID: 2790 RVA: 0x000393AA File Offset: 0x000375AA
		public TimeSpan DefaultRestartRoundDuration { get; set; } = TimeSpan.FromMinutes(2.0);

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x000393B3 File Offset: 0x000375B3
		// (set) Token: 0x06000AE8 RID: 2792 RVA: 0x000393BB File Offset: 0x000375BB
		public TimeSpan? LastCountdownStart { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x000393C4 File Offset: 0x000375C4
		// (set) Token: 0x06000AEA RID: 2794 RVA: 0x000393CC File Offset: 0x000375CC
		public TimeSpan? ExpectedCountdownEnd { get; set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x000393D8 File Offset: 0x000375D8
		public TimeSpan? ExpectedShuttleLength
		{
			get
			{
				TimeSpan? expectedCountdownEnd = this.ExpectedCountdownEnd;
				TimeSpan? lastCountdownStart = this.LastCountdownStart;
				if (!(expectedCountdownEnd != null & lastCountdownStart != null))
				{
					return null;
				}
				return new TimeSpan?(expectedCountdownEnd.GetValueOrDefault() - lastCountdownStart.GetValueOrDefault());
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x00039428 File Offset: 0x00037628
		public TimeSpan? ShuttleTimeLeft
		{
			get
			{
				TimeSpan? expectedCountdownEnd = this.ExpectedCountdownEnd;
				TimeSpan curTime = this._gameTiming.CurTime;
				if (expectedCountdownEnd == null)
				{
					return null;
				}
				return new TimeSpan?(expectedCountdownEnd.GetValueOrDefault() - curTime);
			}
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x0003946D File Offset: 0x0003766D
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(delegate(RoundRestartCleanupEvent _)
			{
				this.Reset();
			}, null, null);
			this.SetAutoCallTime();
		}

		// Token: 0x06000AEE RID: 2798 RVA: 0x0003948F File Offset: 0x0003768F
		private void SetAutoCallTime()
		{
			this.AutoCallStartTime = this._gameTiming.CurTime;
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x000394A4 File Offset: 0x000376A4
		private void Reset()
		{
			if (this._countdownTokenSource != null)
			{
				this._countdownTokenSource.Cancel();
				this._countdownTokenSource = null;
			}
			if (this._cooldownTokenSource != null)
			{
				this._cooldownTokenSource.Cancel();
				this._cooldownTokenSource = null;
			}
			this.LastCountdownStart = null;
			this.ExpectedCountdownEnd = null;
			this.SetAutoCallTime();
			this.AutoCalledBefore = false;
			base.RaiseLocalEvent<RoundEndSystemChangedEvent>(RoundEndSystemChangedEvent.Default);
		}

		// Token: 0x06000AF0 RID: 2800 RVA: 0x0003951B File Offset: 0x0003771B
		public bool CanCallOrRecall()
		{
			return this._cooldownTokenSource == null;
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x00039528 File Offset: 0x00037728
		public void RequestRoundEnd(EntityUid? requester = null, bool checkCooldown = true, bool autoCall = false, EntityUid? player = null)
		{
			TimeSpan duration = this.DefaultCountdownDuration;
			if (requester != null)
			{
				EntityUid? stationUid = this._stationSystem.GetOwningStation(requester.Value, null);
				AlertLevelComponent alertLevel;
				if (base.TryComp<AlertLevelComponent>(stationUid, ref alertLevel))
				{
					duration = this._protoManager.Index<AlertLevelPrototype>("stationAlerts").Levels[alertLevel.CurrentLevel].ShuttleTime;
				}
			}
			this.RequestRoundEnd(duration, requester, checkCooldown, autoCall, player);
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00039598 File Offset: 0x00037798
		public void RequestRoundEnd(TimeSpan countdownTime, EntityUid? requester = null, bool checkCooldown = true, bool autoCall = false, EntityUid? player = null)
		{
			if (this._gameTicker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			if (checkCooldown && this._cooldownTokenSource != null)
			{
				return;
			}
			if (this._countdownTokenSource != null)
			{
				return;
			}
			this._countdownTokenSource = new CancellationTokenSource();
			string ftlstring = (player != null) ? "round-end-system-shuttle-called-announcement-by-who" : "round-end-system-shuttle-called-announcement";
			if (requester != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ShuttleCalled;
				LogImpact impact = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(18, 1);
				logStringHandler.AppendLiteral("Shuttle called by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(requester.Value), "user", "ToPrettyString(requester.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.ShuttleCalled;
				LogImpact impact2 = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(14, 0);
				logStringHandler.AppendLiteral("Shuttle called");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			int time;
			string units;
			if (countdownTime.TotalSeconds < 60.0)
			{
				time = countdownTime.Seconds;
				units = "eta-units-seconds";
			}
			else
			{
				time = countdownTime.Minutes;
				units = "eta-units-minutes";
			}
			if (autoCall)
			{
				this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString("round-end-system-shuttle-auto-called-announcement", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("time", time),
					new ValueTuple<string, object>("units", Loc.GetString(units))
				}), Loc.GetString("Station"), false, null, new Color?(Color.Gold));
			}
			else
			{
				IdCardComponent id;
				this._idCardSystem.TryFindIdCard(player.GetValueOrDefault(), out id);
				string author = (id != null) ? (id.FullName + " (" + id.JobTitle + ")").Trim() : "";
				this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString(ftlstring, new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("time", time),
					new ValueTuple<string, object>("units", Loc.GetString(units)),
					new ValueTuple<string, object>("requester", author)
				}), Loc.GetString("Station"), false, null, new Color?(Color.Gold));
			}
			SoundSystem.Play("/Audio/Announcements/shuttlecalled.ogg", Filter.Broadcast(), null);
			this.LastCountdownStart = new TimeSpan?(this._gameTiming.CurTime);
			this.ExpectedCountdownEnd = new TimeSpan?(this._gameTiming.CurTime + countdownTime);
			Timer.Spawn(countdownTime, new Action(this._shuttle.CallEmergencyShuttle), this._countdownTokenSource.Token);
			this.ActivateCooldown();
			base.RaiseLocalEvent<RoundEndSystemChangedEvent>(RoundEndSystemChangedEvent.Default);
			this.SendRoundStatus("Shuttle called");
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00039834 File Offset: 0x00037A34
		public void CancelRoundEndCountdown(EntityUid? requester = null, bool checkCooldown = true, EntityUid? player = null)
		{
			if (this._gameTicker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			if (checkCooldown && this._cooldownTokenSource != null)
			{
				return;
			}
			if (this._countdownTokenSource == null)
			{
				return;
			}
			this._countdownTokenSource.Cancel();
			this._countdownTokenSource = null;
			string ftlstring = (player != null) ? "round-end-system-shuttle-recalled-announcement-by-who" : "round-end-system-shuttle-recalled-announcement";
			if (requester != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.ShuttleRecalled;
				LogImpact impact = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(20, 1);
				logStringHandler.AppendLiteral("Shuttle recalled by ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(requester.Value), "user", "ToPrettyString(requester.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.ShuttleRecalled;
				LogImpact impact2 = LogImpact.High;
				LogStringHandler logStringHandler = new LogStringHandler(16, 0);
				logStringHandler.AppendLiteral("Shuttle recalled");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			IdCardComponent id;
			this._idCardSystem.TryFindIdCard(player.GetValueOrDefault(), out id);
			string author = (id != null) ? (id.FullName + " (" + id.JobTitle + ")").Trim() : "";
			this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString(ftlstring, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("requester", author)
			}), Loc.GetString("Station"), false, null, new Color?(Color.Gold));
			SoundSystem.Play("/Audio/Announcements/shuttlerecalled.ogg", Filter.Broadcast(), null);
			this.LastCountdownStart = null;
			this.ExpectedCountdownEnd = null;
			this.ActivateCooldown();
			base.RaiseLocalEvent<RoundEndSystemChangedEvent>(RoundEndSystemChangedEvent.Default);
			this.SendRoundStatus("Shuttle recalled");
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x000399D8 File Offset: 0x00037BD8
		private void SendRoundStatus(string status)
		{
			UtkaRoundStatusEvent utkaRoundStatusEvent = new UtkaRoundStatusEvent
			{
				Message = status
			};
			this._utkaSocketWrapper.SendMessageToAll(utkaRoundStatusEvent);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00039A00 File Offset: 0x00037C00
		public void EndRound()
		{
			if (this._gameTicker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			this.LastCountdownStart = null;
			this.ExpectedCountdownEnd = null;
			base.RaiseLocalEvent<RoundEndSystemChangedEvent>(RoundEndSystemChangedEvent.Default);
			this._gameTicker.EndRound("");
			CancellationTokenSource countdownTokenSource = this._countdownTokenSource;
			if (countdownTokenSource != null)
			{
				countdownTokenSource.Cancel();
			}
			this._countdownTokenSource = new CancellationTokenSource();
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("round-end-system-round-restart-eta-announcement", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("minutes", this.DefaultRestartRoundDuration.Minutes)
			}), null);
			Timer.Spawn(this.DefaultRestartRoundDuration, new Action(this.AfterEndRoundRestart), this._countdownTokenSource.Token);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00039ADB File Offset: 0x00037CDB
		private void AfterEndRoundRestart()
		{
			if (this._gameTicker.RunLevel != GameRunLevel.PostRound)
			{
				return;
			}
			this.Reset();
			this._gameTicker.RestartRound();
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00039AFD File Offset: 0x00037CFD
		private void ActivateCooldown()
		{
			CancellationTokenSource cooldownTokenSource = this._cooldownTokenSource;
			if (cooldownTokenSource != null)
			{
				cooldownTokenSource.Cancel();
			}
			this._cooldownTokenSource = new CancellationTokenSource();
			Timer.Spawn(this.DefaultCooldownDuration, delegate()
			{
				this._cooldownTokenSource.Cancel();
				this._cooldownTokenSource = null;
				base.RaiseLocalEvent<RoundEndSystemChangedEvent>(RoundEndSystemChangedEvent.Default);
			}, this._cooldownTokenSource.Token);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00039B40 File Offset: 0x00037D40
		public override void Update(float frameTime)
		{
			int mins = this.AutoCalledBefore ? this._cfg.GetCVar<int>(CCVars.EmergencyShuttleAutoCallExtensionTime) : this._cfg.GetCVar<int>(CCVars.EmergencyShuttleAutoCallTime);
			if (mins != 0 && this._gameTiming.CurTime - this.AutoCallStartTime > TimeSpan.FromMinutes((double)mins))
			{
				if (!this._shuttle.EmergencyShuttleArrived && this.ExpectedCountdownEnd == null)
				{
					this.RequestRoundEnd(null, false, true, null);
					this.AutoCalledBefore = true;
				}
				this.SetAutoCallTime();
			}
		}

		// Token: 0x040006B1 RID: 1713
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040006B2 RID: 1714
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040006B3 RID: 1715
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040006B4 RID: 1716
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040006B5 RID: 1717
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x040006B6 RID: 1718
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x040006B7 RID: 1719
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x040006B8 RID: 1720
		[Dependency]
		private readonly ShuttleSystem _shuttle;

		// Token: 0x040006B9 RID: 1721
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040006BA RID: 1722
		[Dependency]
		private readonly IdCardSystem _idCardSystem;

		// Token: 0x040006BB RID: 1723
		[Dependency]
		private readonly UtkaTCPWrapper _utkaSocketWrapper;

		// Token: 0x040006BF RID: 1727
		[Nullable(2)]
		private CancellationTokenSource _countdownTokenSource;

		// Token: 0x040006C0 RID: 1728
		[Nullable(2)]
		private CancellationTokenSource _cooldownTokenSource;

		// Token: 0x040006C3 RID: 1731
		public TimeSpan AutoCallStartTime;

		// Token: 0x040006C4 RID: 1732
		private bool AutoCalledBefore;
	}
}
