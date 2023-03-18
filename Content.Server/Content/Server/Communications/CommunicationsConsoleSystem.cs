using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.Administration.Logs;
using Content.Server.AlertLevel;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.Interaction;
using Content.Server.Popups;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Systems;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.StationEvents;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Communications;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Communications
{
	// Token: 0x02000630 RID: 1584
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CommunicationsConsoleSystem : EntitySystem
	{
		// Token: 0x060021B8 RID: 8632 RVA: 0x000AF9F8 File Offset: 0x000ADBF8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AlertLevelChangedEvent>(new EntityEventHandler<AlertLevelChangedEvent>(this.OnAlertLevelChanged), null, null);
			base.SubscribeLocalEvent<CommunicationsConsoleComponent, ComponentInit>(delegate(EntityUid _, CommunicationsConsoleComponent comp, ComponentInit _)
			{
				this.UpdateCommsConsoleInterface(comp);
			}, null, null);
			base.SubscribeLocalEvent<RoundEndSystemChangedEvent>(delegate(RoundEndSystemChangedEvent _)
			{
				this.OnGenericBroadcastEvent();
			}, null, null);
			base.SubscribeLocalEvent<AlertLevelDelayFinishedEvent>(delegate(AlertLevelDelayFinishedEvent _)
			{
				this.OnGenericBroadcastEvent();
			}, null, null);
			base.SubscribeLocalEvent<CommunicationsConsoleComponent, CommunicationsConsoleSelectAlertLevelMessage>(new ComponentEventHandler<CommunicationsConsoleComponent, CommunicationsConsoleSelectAlertLevelMessage>(this.OnSelectAlertLevelMessage), null, null);
			base.SubscribeLocalEvent<CommunicationsConsoleComponent, CommunicationsConsoleAnnounceMessage>(new ComponentEventHandler<CommunicationsConsoleComponent, CommunicationsConsoleAnnounceMessage>(this.OnAnnounceMessage), null, null);
			base.SubscribeLocalEvent<CommunicationsConsoleComponent, CommunicationsConsoleCallEmergencyShuttleMessage>(new ComponentEventHandler<CommunicationsConsoleComponent, CommunicationsConsoleCallEmergencyShuttleMessage>(this.OnCallShuttleMessage), null, null);
			base.SubscribeLocalEvent<CommunicationsConsoleComponent, CommunicationsConsoleRecallEmergencyShuttleMessage>(new ComponentEventHandler<CommunicationsConsoleComponent, CommunicationsConsoleRecallEmergencyShuttleMessage>(this.OnRecallShuttleMessage), null, null);
		}

		// Token: 0x060021B9 RID: 8633 RVA: 0x000AFAA8 File Offset: 0x000ADCA8
		public override void Update(float frameTime)
		{
			foreach (CommunicationsConsoleComponent comp in base.EntityQuery<CommunicationsConsoleComponent>(false))
			{
				if (comp.AnnouncementCooldownRemaining >= 0f)
				{
					comp.AnnouncementCooldownRemaining -= frameTime;
				}
				comp.UIUpdateAccumulator += frameTime;
				if (comp.UIUpdateAccumulator >= 5f)
				{
					comp.UIUpdateAccumulator -= 5f;
					BoundUserInterface ui = comp.UserInterface;
					if (ui != null && ui.SubscribedSessions.Count > 0)
					{
						this.UpdateCommsConsoleInterface(comp);
					}
				}
			}
			base.Update(frameTime);
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x000AFB60 File Offset: 0x000ADD60
		private void OnGenericBroadcastEvent()
		{
			foreach (CommunicationsConsoleComponent comp in base.EntityQuery<CommunicationsConsoleComponent>(false))
			{
				this.UpdateCommsConsoleInterface(comp);
			}
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x000AFBB0 File Offset: 0x000ADDB0
		private void OnAlertLevelChanged(AlertLevelChangedEvent args)
		{
			foreach (CommunicationsConsoleComponent comp in base.EntityQuery<CommunicationsConsoleComponent>(true))
			{
				EntityUid? owningStation = this._stationSystem.GetOwningStation(comp.Owner, null);
				if (args.Station == owningStation)
				{
					this.UpdateCommsConsoleInterface(comp);
				}
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x000AFC34 File Offset: 0x000ADE34
		public void UpdateCommsConsoleInterface()
		{
			foreach (CommunicationsConsoleComponent comp in base.EntityQuery<CommunicationsConsoleComponent>(false))
			{
				this.UpdateCommsConsoleInterface(comp);
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x000AFC84 File Offset: 0x000ADE84
		public void UpdateCommsConsoleInterface(CommunicationsConsoleComponent comp)
		{
			EntityUid uid = comp.Owner;
			EntityUid? stationUid = this._stationSystem.GetOwningStation(uid, null);
			List<string> levels = null;
			string currentLevel = null;
			float currentDelay = 0f;
			AlertLevelComponent alertComp;
			if (stationUid != null && base.TryComp<AlertLevelComponent>(stationUid.Value, ref alertComp) && alertComp.AlertLevels != null)
			{
				if (alertComp.IsSelectable)
				{
					levels = new List<string>();
					foreach (KeyValuePair<string, AlertLevelDetail> keyValuePair in alertComp.AlertLevels.Levels)
					{
						string text;
						AlertLevelDetail alertLevelDetail;
						keyValuePair.Deconstruct(out text, out alertLevelDetail);
						string id = text;
						if (alertLevelDetail.Selectable)
						{
							levels.Add(id);
						}
					}
				}
				currentLevel = alertComp.CurrentLevel;
				currentDelay = this._alertLevelSystem.GetAlertLevelDelay(stationUid.Value, alertComp);
			}
			BoundUserInterface userInterface = comp.UserInterface;
			if (userInterface == null)
			{
				return;
			}
			userInterface.SetState(new CommunicationsConsoleInterfaceState(this.CanAnnounce(comp), this.CanCallOrRecall(comp), levels, currentLevel, currentDelay, this._roundEndSystem.ExpectedCountdownEnd), null, true);
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x000AFDA8 File Offset: 0x000ADFA8
		private bool CanAnnounce(CommunicationsConsoleComponent comp)
		{
			return comp.AnnouncementCooldownRemaining <= 0f;
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x000AFDBC File Offset: 0x000ADFBC
		private bool CanUse(EntityUid user, EntityUid console)
		{
			AccessReaderComponent accessReaderComponent;
			return this._interaction.InRangeUnobstructed(console, user, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && (!base.TryComp<AccessReaderComponent>(console, ref accessReaderComponent) || base.HasComp<EmaggedComponent>(console) || this._accessReaderSystem.IsAllowed(user, accessReaderComponent));
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x000AFE0C File Offset: 0x000AE00C
		private bool CanCallOrRecall(CommunicationsConsoleComponent comp)
		{
			if (this._shuttle.EmergencyShuttleArrived || !this._roundEndSystem.CanCallOrRecall())
			{
				return false;
			}
			if (!this._cfg.GetCVar<bool>(CCVars.EmergencyShuttleCallEnabled))
			{
				return false;
			}
			if (this._roundEndSystem.ExpectedCountdownEnd == null)
			{
				return comp.CanCallShuttle;
			}
			float recallThreshold = this._cfg.GetCVar<float>(CCVars.EmergencyRecallTurningPoint);
			TimeSpan? timeSpan = this._roundEndSystem.ShuttleTimeLeft;
			if (timeSpan != null)
			{
				TimeSpan left = timeSpan.GetValueOrDefault();
				timeSpan = this._roundEndSystem.ExpectedShuttleLength;
				if (timeSpan != null)
				{
					TimeSpan expected = timeSpan.GetValueOrDefault();
					return left.TotalSeconds / expected.TotalSeconds >= (double)recallThreshold;
				}
			}
			return false;
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x000AFECC File Offset: 0x000AE0CC
		private void OnSelectAlertLevelMessage(EntityUid uid, CommunicationsConsoleComponent comp, CommunicationsConsoleSelectAlertLevelMessage message)
		{
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid mob = attachedEntity.GetValueOrDefault();
				if (mob.Valid)
				{
					if (!this.CanUse(mob, uid))
					{
						this._popupSystem.PopupCursor(Loc.GetString("comms-console-permission-denied"), message.Session, PopupType.Medium);
						return;
					}
					EntityUid? stationUid = this._stationSystem.GetOwningStation(uid, null);
					if (stationUid != null)
					{
						this._alertLevelSystem.SetLevel(stationUid.Value, message.Level, true, true, false, false, null, null);
					}
					return;
				}
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x000AFF5C File Offset: 0x000AE15C
		private void OnAnnounceMessage(EntityUid uid, CommunicationsConsoleComponent comp, CommunicationsConsoleAnnounceMessage message)
		{
			string msg = (message.Message.Length <= 256) ? message.Message.Trim() : (message.Message.Trim().Substring(0, 256) + "...");
			string author = Loc.GetString("comms-console-announcement-unknown-sender");
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid mob = attachedEntity.GetValueOrDefault();
				if (mob.Valid)
				{
					if (!this.CanAnnounce(comp))
					{
						return;
					}
					if (!this.CanUse(mob, uid))
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-permission-denied"), uid, message.Session, PopupType.Small);
						return;
					}
					IdCardComponent id;
					if (this._idCardSystem.TryFindIdCard(mob, out id))
					{
						author = (id.FullName + " (" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(id.JobTitle ?? string.Empty) + ")").Trim();
					}
				}
			}
			comp.AnnouncementCooldownRemaining = (float)comp.DelayBetweenAnnouncements;
			this.UpdateCommsConsoleInterface(comp);
			string title;
			Loc.TryGetString(comp.AnnouncementDisplayName, ref title);
			if (title == null)
			{
				title = comp.AnnouncementDisplayName;
			}
			msg = string.Concat(new string[]
			{
				msg,
				"\n",
				Loc.GetString("comms-console-announcement-sent-by"),
				" ",
				author
			});
			if (comp.AnnounceGlobal)
			{
				this._chatSystem.DispatchGlobalAnnouncement(msg, title, true, comp.AnnouncementSound, new Color?(comp.AnnouncementColor));
				if (message.Session.AttachedEntity != null)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Chat;
					LogImpact impact = LogImpact.Low;
					LogStringHandler logStringHandler = new LogStringHandler(45, 2);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(message.Session.AttachedEntity.Value), "player", "ToPrettyString(message.Session.AttachedEntity.Value)");
					logStringHandler.AppendLiteral(" has sent the following global announcement: ");
					logStringHandler.AppendFormatted(msg);
					adminLogger.Add(type, impact, ref logStringHandler);
				}
				return;
			}
			this._chatSystem.DispatchStationAnnouncement(uid, msg, title, true, null, new Color?(comp.AnnouncementColor));
			if (message.Session.AttachedEntity != null)
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Chat;
				LogImpact impact2 = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(46, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(message.Session.AttachedEntity.Value), "player", "ToPrettyString(message.Session.AttachedEntity.Value)");
				logStringHandler.AppendLiteral(" has sent the following station announcement: ");
				logStringHandler.AppendFormatted(msg);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x000B01DC File Offset: 0x000AE3DC
		private void OnCallShuttleMessage(EntityUid uid, CommunicationsConsoleComponent comp, CommunicationsConsoleCallEmergencyShuttleMessage message)
		{
			if (!this.CanCallOrRecall(comp))
			{
				return;
			}
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid mob = attachedEntity.GetValueOrDefault();
				if (mob.Valid)
				{
					if (!this.OnStationCallOrRecall(uid))
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-no-connection"), uid, message.Session, PopupType.Small);
						return;
					}
					if (!this.CanUse(mob, uid))
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-permission-denied"), uid, message.Session, PopupType.Small);
						return;
					}
					if (this._meteorEvent.RuleStarted && (float)this._gameTicker.RoundDuration().TotalSeconds < this._meteorEvent._timeUntilCallShuttle)
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-meteor-connection"), uid, message.Session, PopupType.Small);
						return;
					}
					this._roundEndSystem.RequestRoundEnd(new EntityUid?(uid), true, false, new EntityUid?(mob));
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Extreme;
					LogStringHandler logStringHandler = new LogStringHandler(24, 1);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(mob), "player", "ToPrettyString(mob)");
					logStringHandler.AppendLiteral(" has called the shuttle.");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x000B030C File Offset: 0x000AE50C
		private void OnRecallShuttleMessage(EntityUid uid, CommunicationsConsoleComponent comp, CommunicationsConsoleRecallEmergencyShuttleMessage message)
		{
			if (!this.CanCallOrRecall(comp))
			{
				return;
			}
			EntityUid? attachedEntity = message.Session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid mob = attachedEntity.GetValueOrDefault();
				if (mob.Valid)
				{
					if (!this.OnStationCallOrRecall(uid))
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-no-connection"), uid, message.Session, PopupType.Small);
						return;
					}
					if (!this.CanUse(mob, uid))
					{
						this._popupSystem.PopupEntity(Loc.GetString("comms-console-permission-denied"), uid, message.Session, PopupType.Small);
						return;
					}
					this._roundEndSystem.CancelRoundEndCountdown(new EntityUid?(uid), true, new EntityUid?(mob));
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Action;
					LogImpact impact = LogImpact.Extreme;
					LogStringHandler logStringHandler = new LogStringHandler(26, 1);
					logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(mob), "player", "ToPrettyString(mob)");
					logStringHandler.AppendLiteral(" has recalled the shuttle.");
					adminLogger.Add(type, impact, ref logStringHandler);
					return;
				}
			}
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x000B03F0 File Offset: 0x000AE5F0
		private bool OnStationCallOrRecall(EntityUid uid)
		{
			EntityUid parent = base.Transform(uid).ParentUid;
			return base.HasComp<BecomesStationComponent>(parent);
		}

		// Token: 0x040014AB RID: 5291
		[Dependency]
		private readonly AccessReaderSystem _accessReaderSystem;

		// Token: 0x040014AC RID: 5292
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x040014AD RID: 5293
		[Dependency]
		private readonly AlertLevelSystem _alertLevelSystem;

		// Token: 0x040014AE RID: 5294
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x040014AF RID: 5295
		[Dependency]
		private readonly IdCardSystem _idCardSystem;

		// Token: 0x040014B0 RID: 5296
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040014B1 RID: 5297
		[Dependency]
		private readonly RoundEndSystem _roundEndSystem;

		// Token: 0x040014B2 RID: 5298
		[Dependency]
		private readonly ShuttleSystem _shuttle;

		// Token: 0x040014B3 RID: 5299
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040014B4 RID: 5300
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040014B5 RID: 5301
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040014B6 RID: 5302
		[Dependency]
		private readonly MeteorStationEventSchedulerSystem _meteorEvent;

		// Token: 0x040014B7 RID: 5303
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x040014B8 RID: 5304
		private const int MaxMessageLength = 256;

		// Token: 0x040014B9 RID: 5305
		private const float UIUpdateInterval = 5f;
	}
}
