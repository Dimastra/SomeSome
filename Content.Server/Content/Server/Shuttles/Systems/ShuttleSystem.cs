using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Access.Systems;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Communications;
using Content.Server.Doors.Systems;
using Content.Server.GameTicking.Events;
using Content.Server.Popups;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Stunnable;
using Content.Server.UserInterface;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.GameTicking;
using Content.Shared.Parallax;
using Content.Shared.Popups;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Events;
using Content.Shared.Shuttles.Systems;
using Content.Shared.StatusEffect;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Systems
{
	// Token: 0x020001F9 RID: 505
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShuttleSystem : SharedShuttleSystem
	{
		// Token: 0x060009E8 RID: 2536 RVA: 0x00033080 File Offset: 0x00031280
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("shuttles");
			this.InitializeEmergencyConsole();
			this.InitializeEscape();
			this.InitializeFTL();
			this.InitializeIFF();
			this.InitializeImpact();
			base.SubscribeLocalEvent<ShuttleComponent, ComponentAdd>(new ComponentEventHandler<ShuttleComponent, ComponentAdd>(this.OnShuttleAdd), null, null);
			base.SubscribeLocalEvent<ShuttleComponent, ComponentStartup>(new ComponentEventHandler<ShuttleComponent, ComponentStartup>(this.OnShuttleStartup), null, null);
			base.SubscribeLocalEvent<ShuttleComponent, ComponentShutdown>(new ComponentEventHandler<ShuttleComponent, ComponentShutdown>(this.OnShuttleShutdown), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			base.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInit), null, null);
			base.SubscribeLocalEvent<GridFixtureChangeEvent>(new EntityEventHandler<GridFixtureChangeEvent>(this.OnGridFixtureChange), null, null);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00033139 File Offset: 0x00031339
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateEmergencyConsole(frameTime);
			this.UpdateHyperspace(frameTime);
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00033150 File Offset: 0x00031350
		private void OnRoundRestart(RoundRestartCleanupEvent ev)
		{
			this.CleanupEmergencyConsole();
			this.CleanupEmergencyShuttle();
			this.CleanupHyperspace();
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00033164 File Offset: 0x00031364
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownEscape();
			this.ShutdownEmergencyConsole();
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00033178 File Offset: 0x00031378
		private void OnShuttleAdd(EntityUid uid, ShuttleComponent component, ComponentAdd args)
		{
			for (int i = 0; i < component.LinearThrusters.Length; i++)
			{
				component.LinearThrusters[i] = new List<ThrusterComponent>();
			}
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x000331A8 File Offset: 0x000313A8
		private void OnGridFixtureChange(GridFixtureChangeEvent args)
		{
			if (args.NewFixtures.Count == 0)
			{
				return;
			}
			EntityUid uid = args.NewFixtures[0].Body.Owner;
			FixturesComponent manager = base.Comp<FixturesComponent>(uid);
			foreach (Fixture fixture in args.NewFixtures)
			{
				this._physics.SetDensity(uid, fixture, 0.5f, false, manager);
				this._fixtures.SetRestitution(uid, fixture, 0.1f, false, manager);
			}
			this._fixtures.FixtureUpdate(uid, true, manager, null);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00033258 File Offset: 0x00031458
		private void OnGridInit(GridInitializeEvent ev)
		{
			if (base.HasComp<MapComponent>(ev.EntityUid))
			{
				return;
			}
			this.EntityManager.EnsureComponent<ShuttleComponent>(ev.EntityUid);
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0003327C File Offset: 0x0003147C
		private void OnShuttleStartup(EntityUid uid, ShuttleComponent component, ComponentStartup args)
		{
			if (!this.EntityManager.HasComponent<MapGridComponent>(component.Owner))
			{
				return;
			}
			PhysicsComponent physicsComponent;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(component.Owner, ref physicsComponent))
			{
				return;
			}
			if (component.Enabled)
			{
				this.Enable(uid, physicsComponent);
			}
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x000332C4 File Offset: 0x000314C4
		public void Toggle(EntityUid uid, ShuttleComponent component)
		{
			PhysicsComponent physicsComponent;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(component.Owner, ref physicsComponent))
			{
				return;
			}
			component.Enabled = !component.Enabled;
			if (component.Enabled)
			{
				this.Enable(uid, physicsComponent);
				return;
			}
			this.Disable(uid, physicsComponent);
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00033310 File Offset: 0x00031510
		private void Enable(EntityUid uid, PhysicsComponent component)
		{
			FixturesComponent manager = null;
			this._physics.SetBodyType(uid, 8, manager, component, null);
			this._physics.SetBodyStatus(component, 1, true);
			this._physics.SetFixedRotation(uid, false, true, manager, component);
			this._physics.SetLinearDamping(component, 0.05f, true);
			this._physics.SetAngularDamping(component, 0.05f, true);
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00033374 File Offset: 0x00031574
		private void Disable(EntityUid uid, PhysicsComponent component)
		{
			FixturesComponent manager = null;
			this._physics.SetBodyType(uid, 4, manager, component, null);
			this._physics.SetBodyStatus(component, 0, true);
			this._physics.SetFixedRotation(uid, true, true, manager, component);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x000333B4 File Offset: 0x000315B4
		private void OnShuttleShutdown(EntityUid uid, ShuttleComponent component, ComponentShutdown args)
		{
			if (this.EntityManager.GetComponent<MetaDataComponent>(uid).EntityLifeStage >= 4)
			{
				return;
			}
			PhysicsComponent physicsComponent;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(component.Owner, ref physicsComponent))
			{
				return;
			}
			this.Disable(uid, physicsComponent);
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060009F4 RID: 2548 RVA: 0x000333F4 File Offset: 0x000315F4
		// (set) Token: 0x060009F5 RID: 2549 RVA: 0x000333FC File Offset: 0x000315FC
		public bool EmergencyShuttleArrived { get; private set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060009F6 RID: 2550 RVA: 0x00033405 File Offset: 0x00031605
		// (set) Token: 0x060009F7 RID: 2551 RVA: 0x0003340D File Offset: 0x0003160D
		public bool EarlyLaunchAuthorized { get; private set; }

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060009F8 RID: 2552 RVA: 0x00033416 File Offset: 0x00031616
		// (set) Token: 0x060009F9 RID: 2553 RVA: 0x0003341E File Offset: 0x0003161E
		public float TransitTime { get; private set; }

		// Token: 0x060009FA RID: 2554 RVA: 0x00033428 File Offset: 0x00031628
		private void InitializeEmergencyConsole()
		{
			this._configManager.OnValueChanged<float>(CCVars.EmergencyShuttleTransitTime, new Action<float>(this.SetTransitTime), true);
			this._configManager.OnValueChanged<float>(CCVars.EmergencyShuttleAuthorizeTime, new Action<float>(this.SetAuthorizeTime), true);
			base.SubscribeLocalEvent<EmergencyShuttleConsoleComponent, ComponentStartup>(new ComponentEventHandler<EmergencyShuttleConsoleComponent, ComponentStartup>(this.OnEmergencyStartup), null, null);
			base.SubscribeLocalEvent<EmergencyShuttleConsoleComponent, EmergencyShuttleAuthorizeMessage>(new ComponentEventHandler<EmergencyShuttleConsoleComponent, EmergencyShuttleAuthorizeMessage>(this.OnEmergencyAuthorize), null, null);
			base.SubscribeLocalEvent<EmergencyShuttleConsoleComponent, EmergencyShuttleRepealMessage>(new ComponentEventHandler<EmergencyShuttleConsoleComponent, EmergencyShuttleRepealMessage>(this.OnEmergencyRepeal), null, null);
			base.SubscribeLocalEvent<EmergencyShuttleConsoleComponent, EmergencyShuttleRepealAllMessage>(new ComponentEventHandler<EmergencyShuttleConsoleComponent, EmergencyShuttleRepealAllMessage>(this.OnEmergencyRepealAll), null, null);
			base.SubscribeLocalEvent<EmergencyShuttleConsoleComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<EmergencyShuttleConsoleComponent, ActivatableUIOpenAttemptEvent>(this.OnEmergencyOpenAttempt), null, null);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000334D3 File Offset: 0x000316D3
		private void OnEmergencyOpenAttempt(EntityUid uid, EmergencyShuttleConsoleComponent component, ActivatableUIOpenAttemptEvent args)
		{
			if (!this._configManager.GetCVar<bool>(CCVars.EmergencyEarlyLaunchAllowed))
			{
				args.Cancel();
				this._popup.PopupEntity(Loc.GetString("emergency-shuttle-console-no-early-launches"), uid, args.User, PopupType.Small);
			}
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x0003350A File Offset: 0x0003170A
		private void SetAuthorizeTime(float obj)
		{
			this._authorizeTime = obj;
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00033513 File Offset: 0x00031713
		private void SetTransitTime(float obj)
		{
			this.TransitTime = obj;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x0003351C File Offset: 0x0003171C
		private void ShutdownEmergencyConsole()
		{
			this._configManager.UnsubValueChanged<float>(CCVars.EmergencyShuttleAuthorizeTime, new Action<float>(this.SetAuthorizeTime));
			this._configManager.UnsubValueChanged<float>(CCVars.EmergencyShuttleTransitTime, new Action<float>(this.SetTransitTime));
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00033556 File Offset: 0x00031756
		private void OnEmergencyStartup(EntityUid uid, EmergencyShuttleConsoleComponent component, ComponentStartup args)
		{
			this.UpdateConsoleState(uid, component);
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00033560 File Offset: 0x00031760
		private void UpdateEmergencyConsole(float frameTime)
		{
			if (this._consoleAccumulator <= 0f)
			{
				return;
			}
			this._consoleAccumulator -= frameTime;
			if (!this._launchedShuttles && this._consoleAccumulator <= this._authorizeTime && !this.EarlyLaunchAuthorized)
			{
				this.AnnounceLaunch();
			}
			if (!this._launchedShuttles && this._consoleAccumulator <= 5.5f)
			{
				this._launchedShuttles = true;
				if (this.CentComMap != null)
				{
					foreach (StationDataComponent comp in base.EntityQuery<StationDataComponent>(true))
					{
						ShuttleComponent shuttle;
						if (base.TryComp<ShuttleComponent>(comp.EmergencyShuttle, ref shuttle))
						{
							if (base.Deleted(this.CentCom))
							{
								this.FTLTravel(shuttle, new EntityCoordinates(this._mapManager.GetMapEntityId(this.CentComMap.Value), Vector2.One * 1000f), this._consoleAccumulator, this.TransitTime);
							}
							else
							{
								this.FTLTravel(shuttle, this.CentCom.Value, this._consoleAccumulator, this.TransitTime, true);
							}
						}
					}
				}
			}
			if (this._consoleAccumulator <= 0f)
			{
				this._launchedShuttles = true;
				ChatSystem chatSystem = this._chatSystem;
				string text = "emergency-shuttle-left";
				ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
				int num = 0;
				string item = "transitTime";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				defaultInterpolatedStringHandler.AppendFormatted<float>(this.TransitTime, "0");
				array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
				chatSystem.DispatchGlobalAnnouncement(Loc.GetString(text, array), "Central Command", true, null, null);
				this._roundEndCancelToken = new CancellationTokenSource();
				Timer.Spawn((int)(this.TransitTime * 1000f) + this._bufferTime.Milliseconds, delegate()
				{
					this._roundEnd.EndRound();
				}, this._roundEndCancelToken.Token);
				if (this.CentCom != null)
				{
					this.AddFTLDestination(this.CentCom.Value, true);
				}
			}
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00033784 File Offset: 0x00031984
		private void OnEmergencyRepealAll(EntityUid uid, EmergencyShuttleConsoleComponent component, EmergencyShuttleRepealAllMessage args)
		{
			EntityUid? player = args.Session.AttachedEntity;
			if (player == null)
			{
				return;
			}
			if (!this._reader.FindAccessTags(player.Value).Contains("EmergencyShuttleRepealAll"))
			{
				this._popup.PopupCursor(Loc.GetString("emergency-shuttle-console-denied"), player.Value, PopupType.Medium);
				return;
			}
			if (component.AuthorizedEntities.Count == 0)
			{
				return;
			}
			ISharedAdminLogManager logger = this._logger;
			LogType type = LogType.EmergencyShuttle;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(45, 1);
			logStringHandler.AppendLiteral("Emergency shuttle early launch REPEAL ALL by ");
			logStringHandler.AppendFormatted<ICommonSession>(args.Session, "user", "args.Session");
			logger.Add(type, impact, ref logStringHandler);
			this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString("emergency-shuttle-console-auth-revoked", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("remaining", component.AuthorizationsRequired)
			}), "Central Command", true, null, null);
			component.AuthorizedEntities.Clear();
			this.UpdateAllEmergencyConsoles();
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00033888 File Offset: 0x00031A88
		private void OnEmergencyRepeal(EntityUid uid, EmergencyShuttleConsoleComponent component, EmergencyShuttleRepealMessage args)
		{
			EntityUid? player = args.Session.AttachedEntity;
			if (player == null)
			{
				return;
			}
			IdCardComponent idCard;
			if (!this._idSystem.TryFindIdCard(player.Value, out idCard) || !this._reader.IsAllowed(idCard.Owner, uid, null))
			{
				this._popup.PopupCursor(Loc.GetString("emergency-shuttle-console-denied"), player.Value, PopupType.Medium);
				return;
			}
			if (!component.AuthorizedEntities.Remove(base.MetaData(idCard.Owner).EntityName))
			{
				return;
			}
			ISharedAdminLogManager logger = this._logger;
			LogType type = LogType.EmergencyShuttle;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(41, 1);
			logStringHandler.AppendLiteral("Emergency shuttle early launch REPEAL by ");
			logStringHandler.AppendFormatted<ICommonSession>(args.Session, "user", "args.Session");
			logger.Add(type, impact, ref logStringHandler);
			int remaining = component.AuthorizationsRequired - component.AuthorizedEntities.Count;
			this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString("emergency-shuttle-console-auth-revoked", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("remaining", remaining)
			}), "Central Command", true, null, null);
			this.CheckForLaunch(component);
			this.UpdateAllEmergencyConsoles();
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x000339B4 File Offset: 0x00031BB4
		private void OnEmergencyAuthorize(EntityUid uid, EmergencyShuttleConsoleComponent component, EmergencyShuttleAuthorizeMessage args)
		{
			EntityUid? player = args.Session.AttachedEntity;
			if (player == null)
			{
				return;
			}
			IdCardComponent idCard;
			if (!this._idSystem.TryFindIdCard(player.Value, out idCard) || !this._reader.IsAllowed(idCard.Owner, uid, null))
			{
				this._popup.PopupCursor(Loc.GetString("emergency-shuttle-console-denied"), args.Session, PopupType.Medium);
				return;
			}
			if (!component.AuthorizedEntities.Add(base.MetaData(idCard.Owner).EntityName))
			{
				return;
			}
			ISharedAdminLogManager logger = this._logger;
			LogType type = LogType.EmergencyShuttle;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(39, 1);
			logStringHandler.AppendLiteral("Emergency shuttle early launch AUTH by ");
			logStringHandler.AppendFormatted<ICommonSession>(args.Session, "user", "args.Session");
			logger.Add(type, impact, ref logStringHandler);
			int remaining = component.AuthorizationsRequired - component.AuthorizedEntities.Count;
			if (remaining > 0)
			{
				this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString("emergency-shuttle-console-auth-left", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("remaining", remaining)
				}), "Central Command", false, null, new Color?(ShuttleSystem.DangerColor));
			}
			if (!this.CheckForLaunch(component))
			{
				SoundSystem.Play("/Audio/Misc/notice1.ogg", Filter.Broadcast(), null);
			}
			this.UpdateAllEmergencyConsoles();
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00033AFE File Offset: 0x00031CFE
		private void CleanupEmergencyConsole()
		{
			this._announced = false;
			this._roundEndCancelToken = null;
			this._launchedShuttles = false;
			this._consoleAccumulator = 0f;
			this.EarlyLaunchAuthorized = false;
			this.EmergencyShuttleArrived = false;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00033B30 File Offset: 0x00031D30
		private void UpdateAllEmergencyConsoles()
		{
			foreach (EmergencyShuttleConsoleComponent comp in base.EntityQuery<EmergencyShuttleConsoleComponent>(true))
			{
				this.UpdateConsoleState(comp.Owner, comp);
			}
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00033B84 File Offset: 0x00031D84
		private void UpdateConsoleState(EntityUid uid, EmergencyShuttleConsoleComponent component)
		{
			List<string> auths = new List<string>();
			foreach (string auth in component.AuthorizedEntities)
			{
				auths.Add(auth);
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(uid, EmergencyConsoleUiKey.Key, null);
			if (uiOrNull == null)
			{
				return;
			}
			uiOrNull.SetState(new EmergencyConsoleBoundUserInterfaceState
			{
				EarlyLaunchTime = (this.EarlyLaunchAuthorized ? new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds((double)this._consoleAccumulator)) : null),
				Authorizations = auths,
				AuthorizationsRequired = component.AuthorizationsRequired
			}, null, true);
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00033C50 File Offset: 0x00031E50
		private bool CheckForLaunch(EmergencyShuttleConsoleComponent component)
		{
			if (component.AuthorizedEntities.Count < component.AuthorizationsRequired || this.EarlyLaunchAuthorized)
			{
				return false;
			}
			this.EarlyLaunch();
			return true;
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00033C78 File Offset: 0x00031E78
		public bool EarlyLaunch()
		{
			if (this.EarlyLaunchAuthorized || !this.EmergencyShuttleArrived || this._consoleAccumulator <= this._authorizeTime)
			{
				return false;
			}
			ISharedAdminLogManager logger = this._logger;
			LogType type = LogType.EmergencyShuttle;
			LogImpact impact = LogImpact.Extreme;
			LogStringHandler logStringHandler = new LogStringHandler(35, 0);
			logStringHandler.AppendLiteral("Emergency shuttle launch authorized");
			logger.Add(type, impact, ref logStringHandler);
			this._consoleAccumulator = this._authorizeTime;
			this.EarlyLaunchAuthorized = true;
			base.RaiseLocalEvent<EmergencyShuttleAuthorizedEvent>(new EmergencyShuttleAuthorizedEvent());
			this.AnnounceLaunch();
			this.UpdateAllEmergencyConsoles();
			return true;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00033CF8 File Offset: 0x00031EF8
		private void AnnounceLaunch()
		{
			if (this._announced)
			{
				return;
			}
			this._announced = true;
			ChatSystem chatSystem = this._chatSystem;
			string text = "emergency-shuttle-launch-time";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "consoleAccumulator";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(this._consoleAccumulator, "0");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			chatSystem.DispatchGlobalAnnouncement(Loc.GetString(text, array), "Central Command", false, null, new Color?(ShuttleSystem.DangerColor));
			SoundSystem.Play("/Audio/Misc/notice1.ogg", Filter.Broadcast(), null);
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00033D8D File Offset: 0x00031F8D
		public bool DelayEmergencyRoundEnd()
		{
			if (this._roundEndCancelToken == null)
			{
				return false;
			}
			CancellationTokenSource roundEndCancelToken = this._roundEndCancelToken;
			if (roundEndCancelToken != null)
			{
				roundEndCancelToken.Cancel();
			}
			this._roundEndCancelToken = null;
			return true;
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000A0B RID: 2571 RVA: 0x00033DB2 File Offset: 0x00031FB2
		// (set) Token: 0x06000A0C RID: 2572 RVA: 0x00033DBA File Offset: 0x00031FBA
		public MapId? CentComMap { get; private set; }

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000A0D RID: 2573 RVA: 0x00033DC3 File Offset: 0x00031FC3
		// (set) Token: 0x06000A0E RID: 2574 RVA: 0x00033DCB File Offset: 0x00031FCB
		public EntityUid? CentCom { get; private set; }

		// Token: 0x06000A0F RID: 2575 RVA: 0x00033DD4 File Offset: 0x00031FD4
		private void InitializeEscape()
		{
			this._emergencyShuttleEnabled = this._configManager.GetCVar<bool>(CCVars.EmergencyShuttleEnabled);
			this._configManager.OnValueChanged<bool>(CCVars.EmergencyShuttleEnabled, new Action<bool>(this.SetEmergencyShuttleEnabled), false);
			base.SubscribeLocalEvent<RoundStartingEvent>(new EntityEventHandler<RoundStartingEvent>(this.OnRoundStart), null, null);
			base.SubscribeLocalEvent<StationDataComponent, ComponentStartup>(new ComponentEventHandler<StationDataComponent, ComponentStartup>(this.OnStationStartup), null, null);
			base.SubscribeNetworkEvent<EmergencyShuttleRequestPositionMessage>(new EntitySessionEventHandler<EmergencyShuttleRequestPositionMessage>(this.OnShuttleRequestPosition), null, null);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00033E50 File Offset: 0x00032050
		private void SetEmergencyShuttleEnabled(bool value)
		{
			if (this._emergencyShuttleEnabled == value)
			{
				return;
			}
			this._emergencyShuttleEnabled = value;
			if (value)
			{
				this.SetupEmergencyShuttle();
				return;
			}
			this.CleanupEmergencyShuttle();
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00033E73 File Offset: 0x00032073
		private void ShutdownEscape()
		{
			this._configManager.UnsubValueChanged<bool>(CCVars.EmergencyShuttleEnabled, new Action<bool>(this.SetEmergencyShuttleEnabled));
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x00033E94 File Offset: 0x00032094
		private void OnShuttleRequestPosition(EmergencyShuttleRequestPositionMessage msg, EntitySessionEventArgs args)
		{
			if (!this._admin.IsAdmin((IPlayerSession)args.SenderSession, false))
			{
				return;
			}
			EntityUid? player = args.SenderSession.AttachedEntity;
			StationDataComponent stationData;
			ShuttleComponent shuttle;
			if (player == null || !base.TryComp<StationDataComponent>(this._station.GetOwningStation(player.Value, null), ref stationData) || !base.TryComp<ShuttleComponent>(stationData.EmergencyShuttle, ref shuttle))
			{
				return;
			}
			EntityUid? targetGrid = this._station.GetLargestGrid(stationData);
			if (targetGrid == null)
			{
				return;
			}
			ShuttleSystem.DockingConfig config = this.GetDockingConfig(shuttle, targetGrid.Value);
			if (config == null)
			{
				return;
			}
			base.RaiseNetworkEvent(new EmergencyShuttlePositionMessage
			{
				StationUid = targetGrid,
				Position = new Box2?(config.Area)
			});
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00033F50 File Offset: 0x00032150
		private bool ValidSpawn(MapGridComponent grid, Box2 area)
		{
			return !grid.GetLocalTilesIntersecting(area, true, null).Any<TileRef>();
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00033F64 File Offset: 0x00032164
		[return: Nullable(2)]
		private ShuttleSystem.DockingConfig GetDockingConfig(ShuttleComponent component, EntityUid targetGrid)
		{
			List<DockingComponent> gridDocks = this.GetDocks(targetGrid);
			if (gridDocks.Count <= 0)
			{
				return null;
			}
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			MapGridComponent targetGridGrid = base.Comp<MapGridComponent>(targetGrid);
			TransformComponent targetGridXform = xformQuery.GetComponent(targetGrid);
			Angle targetGridAngle = targetGridXform.WorldRotation.Reduced();
			List<DockingComponent> shuttleDocks = this.GetDocks(component.Owner);
			Box2 shuttleAABB = base.Comp<MapGridComponent>(component.Owner).LocalAABB;
			List<ShuttleSystem.DockingConfig> validDockConfigs = new List<ShuttleSystem.DockingConfig>();
			if (shuttleDocks.Count > 0)
			{
				Func<MapGridComponent, bool> <>9__3;
				foreach (DockingComponent shuttleDock in shuttleDocks)
				{
					TransformComponent shuttleDockXform = xformQuery.GetComponent(shuttleDock.Owner);
					foreach (DockingComponent gridDock in gridDocks)
					{
						TransformComponent gridXform = xformQuery.GetComponent(gridDock.Owner);
						Box2? dockedAABB;
						Matrix3 matty;
						Angle targetAngle;
						if (this.CanDock(shuttleDock, shuttleDockXform, gridDock, gridXform, targetGridAngle, shuttleAABB, targetGridGrid, out dockedAABB, out matty, out targetAngle))
						{
							EntityCoordinates spawnPosition;
							spawnPosition..ctor(targetGrid, matty.Transform(Vector2.Zero));
							spawnPosition..ctor(targetGridXform.MapUid.Value, spawnPosition.ToMapPos(this.EntityManager));
							Box2Rotated dockedBounds;
							dockedBounds..ctor(shuttleAABB.Translated(spawnPosition.Position), targetGridAngle, spawnPosition.Position);
							IEnumerable<MapGridComponent> source = this._mapManager.FindGridsIntersecting(targetGridXform.MapID, dockedBounds, false);
							Func<MapGridComponent, bool> predicate;
							if ((predicate = <>9__3) == null)
							{
								predicate = (<>9__3 = ((MapGridComponent o) => o.Owner != targetGrid));
							}
							if (!source.Any(predicate))
							{
								List<ValueTuple<DockingComponent, DockingComponent>> dockedPorts = new List<ValueTuple<DockingComponent, DockingComponent>>
								{
									new ValueTuple<DockingComponent, DockingComponent>(shuttleDock, gridDock)
								};
								foreach (DockingComponent other in shuttleDocks)
								{
									if (other != shuttleDock)
									{
										foreach (DockingComponent otherGrid in gridDocks)
										{
											Box2? otherDockedAABB;
											Matrix3 matrix;
											Angle otherTargetAngle;
											if (otherGrid != gridDock && this.CanDock(other, xformQuery.GetComponent(other.Owner), otherGrid, xformQuery.GetComponent(otherGrid.Owner), targetGridAngle, shuttleAABB, targetGridGrid, out otherDockedAABB, out matrix, out otherTargetAngle) && otherDockedAABB.Equals(dockedAABB) && targetAngle.Equals(otherTargetAngle))
											{
												dockedPorts.Add(new ValueTuple<DockingComponent, DockingComponent>(other, otherGrid));
											}
										}
									}
								}
								validDockConfigs.Add(new ShuttleSystem.DockingConfig
								{
									Docks = dockedPorts,
									Area = dockedAABB.Value,
									Coordinates = spawnPosition,
									Angle = targetAngle
								});
							}
						}
					}
				}
			}
			if (validDockConfigs.Count <= 0)
			{
				return null;
			}
			Func<ValueTuple<DockingComponent, DockingComponent>, bool> <>9__4;
			validDockConfigs = validDockConfigs.OrderByDescending(delegate(ShuttleSystem.DockingConfig x)
			{
				IEnumerable<ValueTuple<DockingComponent, DockingComponent>> docks2 = x.Docks;
				Func<ValueTuple<DockingComponent, DockingComponent>, bool> predicate2;
				if ((predicate2 = <>9__4) == null)
				{
					predicate2 = (<>9__4 = (([TupleElementNames(new string[]
					{
						"DockA",
						"DockB"
					})] ValueTuple<DockingComponent, DockingComponent> docks) => this.HasComp<EmergencyDockComponent>(docks.Item2.Owner)));
				}
				return docks2.Any(predicate2);
			}).ThenByDescending((ShuttleSystem.DockingConfig x) => x.Docks.Count).ThenBy(delegate(ShuttleSystem.DockingConfig x)
			{
				Angle angle = x.Angle.Reduced();
				return Math.Abs(Angle.ShortestDistance(ref angle, ref targetGridAngle).Theta);
			}).ToList<ShuttleSystem.DockingConfig>();
			ShuttleSystem.DockingConfig dockingConfig = validDockConfigs.First<ShuttleSystem.DockingConfig>();
			dockingConfig.TargetGrid = targetGrid;
			return dockingConfig;
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00034340 File Offset: 0x00032540
		public void CallEmergencyShuttle(EntityUid? stationUid)
		{
			StationDataComponent stationData;
			TransformComponent xform;
			ShuttleComponent shuttle;
			if (!base.TryComp<StationDataComponent>(stationUid, ref stationData) || !base.TryComp<TransformComponent>(stationData.EmergencyShuttle, ref xform) || !base.TryComp<ShuttleComponent>(stationData.EmergencyShuttle, ref shuttle))
			{
				return;
			}
			EntityUid? targetGrid = this._station.GetLargestGrid(stationData);
			LogStringHandler logStringHandler;
			if (targetGrid == null)
			{
				ISharedAdminLogManager logger = this._logger;
				LogType type = LogType.EmergencyShuttle;
				LogImpact impact = LogImpact.High;
				logStringHandler = new LogStringHandler(47, 2);
				logStringHandler.AppendLiteral("Emergency shuttle ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(stationUid.Value), "ToPrettyString(stationUid.Value)");
				logStringHandler.AppendLiteral(" unable to dock with station ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(stationUid.Value), "ToPrettyString(stationUid.Value)");
				logger.Add(type, impact, ref logStringHandler);
				this._chatSystem.DispatchStationAnnouncement(stationUid.Value, Loc.GetString("emergency-shuttle-good-luck"), "Central Command", false, null, null);
				SoundSystem.Play("/Audio/Misc/notice1.ogg", Filter.Broadcast(), null);
				return;
			}
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			if (this.TryFTLDock(shuttle, targetGrid.Value))
			{
				TransformComponent targetXform;
				if (base.TryComp<TransformComponent>(targetGrid.Value, ref targetXform))
				{
					Angle angle = this.GetAngle(xform, targetXform, xformQuery);
					ChatSystem chatSystem = this._chatSystem;
					EntityUid value = stationUid.Value;
					string text = "emergency-shuttle-docked";
					ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
					int num = 0;
					string item = "time";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<float>(this._consoleAccumulator, "0");
					array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
					array[1] = new ValueTuple<string, object>("direction", angle.GetDir());
					chatSystem.DispatchStationAnnouncement(value, Loc.GetString(text, array), "Central Command", false, null, null);
				}
				ISharedAdminLogManager logger2 = this._logger;
				LogType type2 = LogType.EmergencyShuttle;
				LogImpact impact2 = LogImpact.High;
				logStringHandler = new LogStringHandler(39, 1);
				logStringHandler.AppendLiteral("Emergency shuttle ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(stationUid.Value), "ToPrettyString(stationUid.Value)");
				logStringHandler.AppendLiteral(" docked with stations");
				logger2.Add(type2, impact2, ref logStringHandler);
				SoundSystem.Play("/Audio/Announcements/shuttle_dock.ogg", Filter.Broadcast(), null);
				return;
			}
			TransformComponent targetXform2;
			if (base.TryComp<TransformComponent>(targetGrid.Value, ref targetXform2))
			{
				Angle angle2 = this.GetAngle(xform, targetXform2, xformQuery);
				this._chatSystem.DispatchStationAnnouncement(stationUid.Value, Loc.GetString("emergency-shuttle-nearby", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("direction", angle2.GetDir())
				}), "Central Command", false, null, null);
			}
			ISharedAdminLogManager logger3 = this._logger;
			LogType type3 = LogType.EmergencyShuttle;
			LogImpact impact3 = LogImpact.High;
			logStringHandler = new LogStringHandler(59, 2);
			logStringHandler.AppendLiteral("Emergency shuttle ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(stationUid.Value), "ToPrettyString(stationUid.Value)");
			logStringHandler.AppendLiteral(" unable to find a valid docking port for ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(stationUid.Value), "ToPrettyString(stationUid.Value)");
			logger3.Add(type3, impact3, ref logStringHandler);
			SoundSystem.Play("/Audio/Misc/notice1.ogg", Filter.Broadcast(), null);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00034650 File Offset: 0x00032850
		private Angle GetAngle(TransformComponent xform, TransformComponent targetXform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery)
		{
			ValueTuple<Vector2, Angle> worldPositionRotation = xform.GetWorldPositionRotation(xformQuery);
			Vector2 shuttlePos = worldPositionRotation.Item1;
			Angle shuttleRot = worldPositionRotation.Item2;
			ValueTuple<Vector2, Angle> worldPositionRotation2 = targetXform.GetWorldPositionRotation(xformQuery);
			Vector2 targetPos = worldPositionRotation2.Item1;
			Angle targetRot = worldPositionRotation2.Item2;
			Transform transform = new Transform(shuttlePos, shuttleRot);
			Vector2 localCenter = base.Comp<PhysicsComponent>(xform.Owner).LocalCenter;
			Vector2 vector = Transform.Mul(ref transform, ref localCenter);
			transform = new Transform(targetPos, targetRot);
			localCenter = base.Comp<PhysicsComponent>(targetXform.Owner).LocalCenter;
			Vector2 targetCOM = Transform.Mul(ref transform, ref localCenter);
			Vector2 vector2 = vector - targetCOM;
			Angle targetRotation = targetRot;
			return DirectionExtensions.ToWorldAngle(vector2) - targetRotation;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x000346E8 File Offset: 0x000328E8
		private bool CanDock(DockingComponent shuttleDock, TransformComponent shuttleDockXform, DockingComponent gridDock, TransformComponent gridDockXform, Angle targetGridRotation, Box2 shuttleAABB, MapGridComponent grid, [NotNullWhen(true)] out Box2? shuttleDockedAABB, out Matrix3 matty, out Angle gridRotation)
		{
			gridRotation = Angle.Zero;
			matty = Matrix3.Identity;
			shuttleDockedAABB = null;
			if (shuttleDock.Docked || gridDock.Docked || !shuttleDockXform.Anchored || !gridDockXform.Anchored)
			{
				return false;
			}
			Vector2 localPosition = shuttleDockXform.LocalPosition;
			Angle localRotation = shuttleDockXform.LocalRotation;
			Vector2 vector = new Vector2(0f, -1f);
			Vector2 stationDockPos = localPosition + localRotation.RotateVec(ref vector);
			Angle shuttleDockAngle = shuttleDockXform.LocalRotation;
			Angle gridDockAngle = gridDockXform.LocalRotation.Opposite();
			Matrix3 stationDockMatrix = Matrix3.CreateInverseTransform(ref stationDockPos, ref shuttleDockAngle);
			vector = gridDockXform.LocalPosition;
			Matrix3 gridXformMatrix = Matrix3.CreateTransform(ref vector, ref gridDockAngle);
			Matrix3.Multiply(ref stationDockMatrix, ref gridXformMatrix, ref matty);
			shuttleDockedAABB = new Box2?(matty.TransformBox(ref shuttleAABB));
			shuttleDockedAABB = new Box2?(shuttleDockedAABB.Value.Enlarged(-0.01f));
			if (!this.ValidSpawn(grid, shuttleDockedAABB.Value))
			{
				return false;
			}
			gridRotation = targetGridRotation + gridDockAngle - shuttleDockAngle;
			return true;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x00034806 File Offset: 0x00032A06
		private void OnStationStartup(EntityUid uid, StationDataComponent component, ComponentStartup args)
		{
			this.AddEmergencyShuttle(component);
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x0003480F File Offset: 0x00032A0F
		private void OnRoundStart(RoundStartingEvent ev)
		{
			this.SetupEmergencyShuttle();
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x00034818 File Offset: 0x00032A18
		public void CallEmergencyShuttle()
		{
			if (this.EmergencyShuttleArrived)
			{
				return;
			}
			if (!this._emergencyShuttleEnabled)
			{
				this._roundEnd.EndRound();
				return;
			}
			this._consoleAccumulator = this._configManager.GetCVar<float>(CCVars.EmergencyShuttleDockTime);
			this.EmergencyShuttleArrived = true;
			if (this.CentComMap != null)
			{
				this._mapManager.SetMapPaused(this.CentComMap.Value, false);
			}
			foreach (StationDataComponent comp in base.EntityQuery<StationDataComponent>(true))
			{
				this.CallEmergencyShuttle(new EntityUid?(comp.Owner));
			}
			this._commsConsole.UpdateCommsConsoleInterface();
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x000348E0 File Offset: 0x00032AE0
		public List<DockingComponent> GetDocks(EntityUid uid)
		{
			List<DockingComponent> result = new List<DockingComponent>();
			foreach (ValueTuple<DockingComponent, TransformComponent> valueTuple in base.EntityQuery<DockingComponent, TransformComponent>(true))
			{
				DockingComponent dock = valueTuple.Item1;
				if (!(valueTuple.Item2.ParentUid != uid) && dock.Enabled)
				{
					result.Add(dock);
				}
			}
			return result;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00034958 File Offset: 0x00032B58
		private void SetupEmergencyShuttle()
		{
			if (!this._emergencyShuttleEnabled || (this.CentComMap != null && this._mapManager.MapExists(this.CentComMap.Value)))
			{
				return;
			}
			this.CentComMap = new MapId?(this._mapManager.CreateMap(null));
			this._mapManager.SetMapPaused(this.CentComMap.Value, true);
			if (!string.IsNullOrEmpty(this._configManager.GetCVar<string>(CCVars.CentcommMap)))
			{
				EntityUid? centcomm = this._map.LoadGrid(this.CentComMap.Value, "/Maps/centcomm.yml", null);
				this.CentCom = centcomm;
				if (this.CentCom != null)
				{
					this.AddFTLDestination(this.CentCom.Value, false);
				}
			}
			else
			{
				this._sawmill.Info("No CentCom map found, skipping setup.");
			}
			foreach (StationDataComponent comp in base.EntityQuery<StationDataComponent>(true))
			{
				this.AddEmergencyShuttle(comp);
			}
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00034A8C File Offset: 0x00032C8C
		private void AddEmergencyShuttle(StationDataComponent component)
		{
			if (!this._emergencyShuttleEnabled || this.CentComMap == null || component.EmergencyShuttle != null || component.StationConfig == null)
			{
				return;
			}
			ResourcePath shuttlePath = component.StationConfig.EmergencyShuttlePath;
			EntityUid? shuttle = this._map.LoadGrid(this.CentComMap.Value, shuttlePath.ToString(), new MapLoadOptions
			{
				Offset = new Vector2(500f + this._shuttleIndex, 0f)
			});
			if (shuttle == null)
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Unable to spawn emergency shuttle ");
				defaultInterpolatedStringHandler.AppendFormatted<ResourcePath>(shuttlePath);
				defaultInterpolatedStringHandler.AppendLiteral(" for ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(component.Owner));
				sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			this._shuttleIndex += this._mapManager.GetGrid(shuttle.Value).LocalAABB.Width + 1f;
			component.EmergencyShuttle = shuttle;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00034BA8 File Offset: 0x00032DA8
		private void CleanupEmergencyShuttle()
		{
			if (this._launchedShuttles)
			{
				this._roundEnd.EndRound();
			}
			this._shuttleIndex = 0f;
			if (this.CentComMap == null || !this._mapManager.MapExists(this.CentComMap.Value))
			{
				this.CentComMap = null;
				return;
			}
			this._mapManager.DeleteMap(this.CentComMap.Value);
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00034C27 File Offset: 0x00032E27
		private void InitializeFTL()
		{
			base.SubscribeLocalEvent<StationGridAddedEvent>(new EntityEventHandler<StationGridAddedEvent>(this.OnStationGridAdd), null, null);
			base.SubscribeLocalEvent<FTLDestinationComponent, EntityPausedEvent>(new ComponentEventRefHandler<FTLDestinationComponent, EntityPausedEvent>(this.OnDestinationPause), null, null);
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00034C51 File Offset: 0x00032E51
		private void OnDestinationPause(EntityUid uid, FTLDestinationComponent component, ref EntityPausedEvent args)
		{
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00034C60 File Offset: 0x00032E60
		private void OnStationGridAdd(StationGridAddedEvent ev)
		{
			PhysicsComponent body;
			if (base.TryComp<PhysicsComponent>(ev.GridId, ref body) && body.Mass > 500f)
			{
				this.AddFTLDestination(ev.GridId, true);
			}
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00034C98 File Offset: 0x00032E98
		[NullableContext(2)]
		public bool CanFTL(EntityUid? uid, [NotNullWhen(false)] out string reason, TransformComponent xform = null)
		{
			reason = null;
			MapGridComponent grid;
			if (!base.TryComp<MapGridComponent>(uid, ref grid) || !base.Resolve<TransformComponent>(uid.Value, ref xform, true))
			{
				return true;
			}
			Matrix3 worldMatrix = xform.WorldMatrix;
			Box2 localAABB = grid.LocalAABB;
			Box2 bounds = worldMatrix.TransformBox(ref localAABB).Enlarged(100f);
			EntityQuery<PhysicsComponent> bodyQuery = base.GetEntityQuery<PhysicsComponent>();
			foreach (MapGridComponent other in this._mapManager.FindGridsIntersecting(xform.MapID, bounds, false))
			{
				PhysicsComponent body;
				if (!(grid.Owner == other.Owner) && bodyQuery.TryGetComponent(other.Owner, ref body) && body.Mass >= 300f)
				{
					reason = Loc.GetString("shuttle-console-proximity");
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00034D8C File Offset: 0x00032F8C
		public FTLDestinationComponent AddFTLDestination(EntityUid uid, bool enabled)
		{
			FTLDestinationComponent destination;
			if (base.TryComp<FTLDestinationComponent>(uid, ref destination) && destination.Enabled == enabled)
			{
				return destination;
			}
			destination = base.EnsureComp<FTLDestinationComponent>(uid);
			if (base.HasComp<FTLComponent>(uid))
			{
				enabled = false;
			}
			destination.Enabled = enabled;
			this._console.RefreshShuttleConsoles();
			return destination;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00034DD6 File Offset: 0x00032FD6
		public void RemoveFTLDestination(EntityUid uid)
		{
			if (!base.RemComp<FTLDestinationComponent>(uid))
			{
				return;
			}
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00034DF0 File Offset: 0x00032FF0
		public void FTLTravel(ShuttleComponent component, EntityCoordinates coordinates, float startupTime = 5.5f, float hyperspaceTime = 30f)
		{
			FTLComponent hyperspace;
			if (!this.TrySetupFTL(component, out hyperspace))
			{
				return;
			}
			hyperspace.StartupTime = startupTime;
			hyperspace.TravelTime = hyperspaceTime;
			hyperspace.Accumulator = hyperspace.StartupTime;
			hyperspace.TargetCoordinates = coordinates;
			hyperspace.Dock = false;
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00034E40 File Offset: 0x00033040
		public void FTLTravel(ShuttleComponent component, EntityUid target, float startupTime = 5.5f, float hyperspaceTime = 30f, bool dock = false)
		{
			FTLComponent hyperspace;
			if (!this.TrySetupFTL(component, out hyperspace))
			{
				return;
			}
			hyperspace.StartupTime = startupTime;
			hyperspace.TravelTime = hyperspaceTime;
			hyperspace.Accumulator = hyperspace.StartupTime;
			hyperspace.TargetUid = new EntityUid?(target);
			hyperspace.Dock = dock;
			this._console.RefreshShuttleConsoles();
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00034E94 File Offset: 0x00033094
		private bool TrySetupFTL(ShuttleComponent shuttle, [Nullable(2)] [NotNullWhen(true)] out FTLComponent component)
		{
			EntityUid uid = shuttle.Owner;
			component = null;
			if (base.HasComp<FTLComponent>(uid))
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Tried queuing ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
				defaultInterpolatedStringHandler.AppendLiteral(" which already has HyperspaceComponent?");
				sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			FTLDestinationComponent dest;
			if (base.TryComp<FTLDestinationComponent>(uid, ref dest))
			{
				dest.Enabled = false;
			}
			this._thruster.DisableLinearThrusters(shuttle);
			this._thruster.EnableLinearThrustDirection(shuttle, 4);
			this._thruster.SetAngularThrust(shuttle, false);
			this.SetDocks(uid, false);
			component = base.AddComp<FTLComponent>(uid);
			component.State = FTLState.Starting;
			SoundSystem.Play(this._startupSound.GetSound(null, null), Filter.Empty().AddInRange(base.Transform(uid).MapPosition, this.GetSoundRange(component.Owner), null, null), new AudioParams?(this._startupSound.Params));
			this.SetupHyperspace();
			return true;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00034F98 File Offset: 0x00033198
		private void UpdateHyperspace(float frameTime)
		{
			foreach (FTLComponent comp in base.EntityQuery<FTLComponent>(false))
			{
				comp.Accumulator -= frameTime;
				if (comp.Accumulator <= 0f)
				{
					TransformComponent xform = base.Transform(comp.Owner);
					FTLState state = comp.State;
					if (state <= FTLState.Travelling)
					{
						if (state == FTLState.Starting)
						{
							this.DoTheDinosaur(xform);
							comp.State = FTLState.Travelling;
							float width = base.Comp<MapGridComponent>(comp.Owner).LocalAABB.Width;
							xform.Coordinates = new EntityCoordinates(this._mapManager.GetMapEntityId(this._hyperSpaceMap.Value), new Vector2(this._index + width / 2f, 0f));
							xform.LocalRotation = Angle.Zero;
							this._index += width + 5f;
							comp.Accumulator += comp.TravelTime - 5f;
							PhysicsComponent body;
							if (base.TryComp<PhysicsComponent>(comp.Owner, ref body))
							{
								this._physics.SetLinearVelocity(comp.Owner, new Vector2(0f, 20f), true, true, null, body);
								this._physics.SetAngularVelocity(comp.Owner, 0f, true, null, body);
								this._physics.SetLinearDamping(body, 0f, true);
								this._physics.SetAngularDamping(body, 0f, true);
							}
							if (comp.TravelSound != null)
							{
								comp.TravelStream = SoundSystem.Play(comp.TravelSound.GetSound(null, null), Filter.Pvs(comp.Owner, 4f, this.EntityManager, null, null), new AudioParams?(comp.TravelSound.Params));
							}
							this.SetDockBolts(comp.Owner, true);
							this._console.RefreshShuttleConsoles(comp.Owner);
							continue;
						}
						if (state == FTLState.Travelling)
						{
							comp.Accumulator += 5f;
							comp.State = FTLState.Arriving;
							ShuttleComponent shuttle;
							if (base.TryComp<ShuttleComponent>(comp.Owner, ref shuttle))
							{
								this._thruster.DisableLinearThrusters(shuttle);
								this._thruster.EnableLinearThrustDirection(shuttle, 1);
							}
							this._console.RefreshShuttleConsoles(comp.Owner);
							continue;
						}
					}
					else
					{
						if (state == FTLState.Arriving)
						{
							this.DoTheDinosaur(xform);
							this.SetDockBolts(comp.Owner, false);
							this.SetDocks(comp.Owner, true);
							PhysicsComponent body;
							if (base.TryComp<PhysicsComponent>(comp.Owner, ref body))
							{
								this._physics.SetLinearVelocity(comp.Owner, Vector2.Zero, true, true, null, body);
								this._physics.SetAngularVelocity(comp.Owner, 0f, true, null, body);
								this._physics.SetLinearDamping(body, 0.05f, true);
								this._physics.SetAngularDamping(body, 0.05f, true);
							}
							ShuttleComponent shuttle;
							base.TryComp<ShuttleComponent>(comp.Owner, ref shuttle);
							if (comp.TargetUid != null && shuttle != null)
							{
								if (comp.Dock)
								{
									this.TryFTLDock(shuttle, comp.TargetUid.Value);
								}
								else
								{
									this.TryFTLProximity(shuttle, comp.TargetUid.Value, null, null);
								}
							}
							else
							{
								xform.Coordinates = comp.TargetCoordinates;
							}
							if (shuttle != null)
							{
								this._thruster.DisableLinearThrusters(shuttle);
							}
							if (comp.TravelStream != null)
							{
								IPlayingAudioStream travelStream = comp.TravelStream;
								if (travelStream != null)
								{
									travelStream.Stop();
								}
								comp.TravelStream = null;
							}
							SoundSystem.Play(this._arrivalSound.GetSound(null, null), Filter.Empty().AddInRange(base.Transform(comp.Owner).MapPosition, this.GetSoundRange(comp.Owner), null, null), new AudioParams?(this._arrivalSound.Params));
							FTLDestinationComponent dest;
							if (base.TryComp<FTLDestinationComponent>(comp.Owner, ref dest))
							{
								dest.Enabled = true;
							}
							comp.State = FTLState.Cooldown;
							comp.Accumulator += 30f;
							this._console.RefreshShuttleConsoles(comp.Owner);
							base.RaiseLocalEvent<HyperspaceJumpCompletedEvent>(new HyperspaceJumpCompletedEvent());
							continue;
						}
						if (state == FTLState.Cooldown)
						{
							base.RemComp<FTLComponent>(comp.Owner);
							this._console.RefreshShuttleConsoles(comp.Owner);
							continue;
						}
					}
					ISawmill sawmill = this._sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Found invalid FTL state ");
					defaultInterpolatedStringHandler.AppendFormatted<FTLState>(comp.State);
					defaultInterpolatedStringHandler.AppendLiteral(" for ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(comp.Owner);
					sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					base.RemComp<FTLComponent>(comp.Owner);
				}
			}
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00035458 File Offset: 0x00033658
		private void SetDocks(EntityUid uid, bool enabled)
		{
			foreach (ValueTuple<DockingComponent, TransformComponent> valueTuple in base.EntityQuery<DockingComponent, TransformComponent>(true))
			{
				DockingComponent dock = valueTuple.Item1;
				if (!(valueTuple.Item2.ParentUid != uid) && dock.Enabled != enabled)
				{
					this._dockSystem.Undock(dock);
					dock.Enabled = enabled;
				}
			}
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x000354D4 File Offset: 0x000336D4
		private void SetDockBolts(EntityUid uid, bool enabled)
		{
			foreach (ValueTuple<DockingComponent, AirlockComponent, TransformComponent> valueTuple in base.EntityQuery<DockingComponent, AirlockComponent, TransformComponent>(true))
			{
				AirlockComponent door = valueTuple.Item2;
				if (!(valueTuple.Item3.ParentUid != uid))
				{
					this._doors.TryClose(door.Owner, null, null, false);
					this._airlock.SetBoltsWithAudio(door.Owner, door, enabled);
				}
			}
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00035564 File Offset: 0x00033764
		private float GetSoundRange(EntityUid uid)
		{
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(uid), ref grid))
			{
				return 4f;
			}
			return MathF.Max(grid.LocalAABB.Width, grid.LocalAABB.Height) + 12.5f;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x000355B4 File Offset: 0x000337B4
		private void SetupHyperspace()
		{
			if (this._hyperSpaceMap != null)
			{
				return;
			}
			this._hyperSpaceMap = new MapId?(this._mapManager.CreateMap(null));
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Setup hyperspace map at ");
			defaultInterpolatedStringHandler.AppendFormatted<MapId>(this._hyperSpaceMap.Value);
			sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			base.EnsureComp<ParallaxComponent>(this._mapManager.GetMapEntityId(this._hyperSpaceMap.Value)).Parallax = "FastSpace";
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00035650 File Offset: 0x00033850
		private void CleanupHyperspace()
		{
			this._index = 0f;
			if (this._hyperSpaceMap == null || !this._mapManager.MapExists(this._hyperSpaceMap.Value))
			{
				this._hyperSpaceMap = null;
				return;
			}
			this._mapManager.DeleteMap(this._hyperSpaceMap.Value);
			this._hyperSpaceMap = null;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x000356BC File Offset: 0x000338BC
		private void DoTheDinosaur(TransformComponent xform)
		{
			EntityQuery<BuckleComponent> buckleQuery = base.GetEntityQuery<BuckleComponent>();
			EntityQuery<StatusEffectsComponent> statusQuery = base.GetEntityQuery<StatusEffectsComponent>();
			ValueList<EntityUid> toKnock = default(ValueList<EntityUid>);
			this.KnockOverKids(xform, buckleQuery, statusQuery, ref toKnock);
			foreach (EntityUid child in toKnock)
			{
				StatusEffectsComponent status;
				if (statusQuery.TryGetComponent(child, ref status))
				{
					this._stuns.TryParalyze(child, this._hyperspaceKnockdownTime, true, status);
				}
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0003574C File Offset: 0x0003394C
		[NullableContext(0)]
		private void KnockOverKids([Nullable(1)] TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<BuckleComponent> buckleQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<StatusEffectsComponent> statusQuery, ref ValueList<EntityUid> toKnock)
		{
			EntityUid? child;
			while (xform.ChildEnumerator.MoveNext(ref child))
			{
				BuckleComponent buckle;
				if (buckleQuery.TryGetComponent(child.Value, ref buckle) && !buckle.Buckled)
				{
					toKnock.Add(child.Value);
				}
			}
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00035798 File Offset: 0x00033998
		public bool TryFTLDock(ShuttleComponent component, EntityUid targetUid)
		{
			TransformComponent xform;
			TransformComponent targetXform;
			if (!base.TryComp<TransformComponent>(component.Owner, ref xform) || !base.TryComp<TransformComponent>(targetUid, ref targetXform) || targetXform.MapUid == null || !targetXform.MapUid.Value.IsValid())
			{
				return false;
			}
			ShuttleSystem.DockingConfig config = this.GetDockingConfig(component, targetUid);
			if (config != null)
			{
				xform.Coordinates = config.Coordinates;
				xform.WorldRotation = config.Angle;
				foreach (ValueTuple<DockingComponent, DockingComponent> valueTuple in config.Docks)
				{
					DockingComponent dockA = valueTuple.Item1;
					DockingComponent dockB = valueTuple.Item2;
					this._dockSystem.Dock(dockA, dockB);
				}
				return true;
			}
			this.TryFTLProximity(component, targetUid, xform, targetXform);
			return false;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0003587C File Offset: 0x00033A7C
		[NullableContext(2)]
		public bool TryFTLProximity([Nullable(1)] ShuttleComponent component, EntityUid targetUid, TransformComponent xform = null, TransformComponent targetXform = null)
		{
			if (!base.Resolve<TransformComponent>(targetUid, ref targetXform, true) || targetXform.MapUid == null || !targetXform.MapUid.Value.IsValid() || !base.Resolve<TransformComponent>(component.Owner, ref xform, true))
			{
				return false;
			}
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			Box2 shuttleAABB = base.Comp<MapGridComponent>(component.Owner).LocalAABB;
			MapGridComponent targetGrid;
			Box2 targetLocalAABB;
			if (base.TryComp<MapGridComponent>(targetUid, ref targetGrid))
			{
				targetLocalAABB = targetGrid.LocalAABB;
			}
			else
			{
				targetLocalAABB = default(Box2);
			}
			Box2 box = this._transform.GetWorldMatrix(targetXform, xformQuery).TransformBox(ref targetLocalAABB);
			Box2 targetAABB = box.Enlarged(shuttleAABB.Size.Length);
			HashSet<EntityUid> nearbyGrids = new HashSet<EntityUid>(1)
			{
				targetUid
			};
			int iteration = 0;
			int lastCount = 1;
			MapId mapId = targetXform.MapID;
			while (iteration < 3)
			{
				foreach (MapGridComponent grid in this._mapManager.FindGridsIntersecting(mapId, targetAABB, false))
				{
					if (nearbyGrids.Add(grid.Owner))
					{
						Matrix3 worldMatrix = this._transform.GetWorldMatrix(grid.Owner, xformQuery);
						box = base.Comp<MapGridComponent>(grid.Owner).LocalAABB;
						Box2 box2 = worldMatrix.TransformBox(ref box);
						targetAABB = targetAABB.Union(ref box2);
					}
				}
				if (nearbyGrids.Count == lastCount)
				{
					break;
				}
				targetAABB = targetAABB.Enlarged(shuttleAABB.Size.Length / 2f);
				iteration++;
				lastCount = nearbyGrids.Count;
				if (iteration == 3)
				{
					using (IEnumerator<MapGridComponent> enumerator = this._mapManager.GetAllGrids().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MapGridComponent grid2 = enumerator.Current;
							if (!nearbyGrids.Contains(grid2.Owner))
							{
								Matrix3 worldMatrix = this._transform.GetWorldMatrix(grid2.Owner, xformQuery);
								box = base.Comp<MapGridComponent>(grid2.Owner).LocalAABB;
								Box2 box2 = worldMatrix.TransformBox(ref box);
								targetAABB = targetAABB.Union(ref box2);
							}
						}
						break;
					}
				}
			}
			float minRadius = (MathF.Max(targetAABB.Width, targetAABB.Height) + MathF.Max(shuttleAABB.Width, shuttleAABB.Height)) / 2f;
			Vector2 spawnPos = targetAABB.Center + this._random.NextVector2(minRadius, minRadius + 64f);
			PhysicsComponent shuttleBody;
			if (base.TryComp<PhysicsComponent>(component.Owner, ref shuttleBody))
			{
				this._physics.SetLinearVelocity(component.Owner, Vector2.Zero, true, true, null, shuttleBody);
				this._physics.SetAngularVelocity(component.Owner, 0f, true, null, shuttleBody);
			}
			xform.Coordinates = new EntityCoordinates(targetXform.MapUid.Value, spawnPos);
			xform.WorldRotation = this._random.NextAngle();
			return true;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00035B9C File Offset: 0x00033D9C
		private void InitializeIFF()
		{
			base.SubscribeLocalEvent<IFFConsoleComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<IFFConsoleComponent, AnchorStateChangedEvent>(this.OnIFFConsoleAnchor), null, null);
			base.SubscribeLocalEvent<IFFConsoleComponent, IFFShowIFFMessage>(new ComponentEventHandler<IFFConsoleComponent, IFFShowIFFMessage>(this.OnIFFShow), null, null);
			base.SubscribeLocalEvent<IFFConsoleComponent, IFFShowVesselMessage>(new ComponentEventHandler<IFFConsoleComponent, IFFShowVesselMessage>(this.OnIFFShowVessel), null, null);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00035BDC File Offset: 0x00033DDC
		private void OnIFFShow(EntityUid uid, IFFConsoleComponent component, IFFShowIFFMessage args)
		{
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(uid, ref xform) || xform.GridUid == null || (component.AllowedFlags & IFFFlags.HideLabel) == IFFFlags.None)
			{
				return;
			}
			if (!args.Show)
			{
				base.AddIFFFlag(xform.GridUid.Value, IFFFlags.HideLabel, null);
				return;
			}
			base.RemoveIFFFlag(xform.GridUid.Value, IFFFlags.HideLabel, null);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00035C44 File Offset: 0x00033E44
		private void OnIFFShowVessel(EntityUid uid, IFFConsoleComponent component, IFFShowVesselMessage args)
		{
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(uid, ref xform) || xform.GridUid == null || (component.AllowedFlags & IFFFlags.Hide) == IFFFlags.None)
			{
				return;
			}
			if (!args.Show)
			{
				base.AddIFFFlag(xform.GridUid.Value, IFFFlags.Hide, null);
				return;
			}
			base.RemoveIFFFlag(xform.GridUid.Value, IFFFlags.Hide, null);
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00035CAC File Offset: 0x00033EAC
		private void OnIFFConsoleAnchor(EntityUid uid, IFFConsoleComponent component, ref AnchorStateChangedEvent args)
		{
			TransformComponent xform;
			IFFComponent iff;
			if (!args.Anchored || !base.TryComp<TransformComponent>(uid, ref xform) || !base.TryComp<IFFComponent>(xform.GridUid, ref iff))
			{
				this._uiSystem.TrySetUiState(uid, IFFConsoleUiKey.Key, new IFFConsoleBoundUserInterfaceState
				{
					AllowedFlags = component.AllowedFlags,
					Flags = IFFFlags.None
				}, null, null, true);
				return;
			}
			this._uiSystem.TrySetUiState(uid, IFFConsoleUiKey.Key, new IFFConsoleBoundUserInterfaceState
			{
				AllowedFlags = component.AllowedFlags,
				Flags = iff.Flags
			}, null, null, true);
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00035D40 File Offset: 0x00033F40
		protected override void UpdateIFFInterfaces(EntityUid gridUid, IFFComponent component)
		{
			base.UpdateIFFInterfaces(gridUid, component);
			foreach (ValueTuple<IFFConsoleComponent, TransformComponent> valueTuple in base.EntityQuery<IFFConsoleComponent, TransformComponent>(true))
			{
				IFFConsoleComponent comp = valueTuple.Item1;
				EntityUid? gridUid2 = valueTuple.Item2.GridUid;
				if (gridUid2 != null && (gridUid2 == null || !(gridUid2.GetValueOrDefault() != gridUid)))
				{
					this._uiSystem.TrySetUiState(comp.Owner, IFFConsoleUiKey.Key, new IFFConsoleBoundUserInterfaceState
					{
						AllowedFlags = comp.AllowedFlags,
						Flags = component.Flags
					}, null, null, true);
				}
			}
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x00035E04 File Offset: 0x00034004
		private void InitializeImpact()
		{
			base.SubscribeLocalEvent<ShuttleComponent, StartCollideEvent>(new ComponentEventRefHandler<ShuttleComponent, StartCollideEvent>(this.OnShuttleCollide), null, null);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00035E1C File Offset: 0x0003401C
		private void OnShuttleCollide(EntityUid uid, ShuttleComponent component, ref StartCollideEvent args)
		{
			PhysicsComponent ourBody = args.OurFixture.Body;
			PhysicsComponent otherBody = args.OtherFixture.Body;
			if (!base.HasComp<ShuttleComponent>(otherBody.Owner))
			{
				return;
			}
			TransformComponent ourXform = base.Transform(ourBody.Owner);
			if (ourXform.MapUid == null)
			{
				return;
			}
			TransformComponent otherXform = base.Transform(otherBody.Owner);
			Vector2 ourPoint = ourXform.InvWorldMatrix.Transform(args.WorldPoint);
			Vector2 otherPoint = otherXform.InvWorldMatrix.Transform(args.WorldPoint);
			Vector2 linearVelocity = this._physics.GetLinearVelocity(ourBody.Owner, ourPoint, ourBody, ourXform);
			Vector2 otherVelocity = this._physics.GetLinearVelocity(otherBody.Owner, otherPoint, otherBody, otherXform);
			float jungleDiff = (linearVelocity - otherVelocity).Length;
			if (jungleDiff < 10f)
			{
				return;
			}
			EntityCoordinates coordinates;
			coordinates..ctor(ourXform.MapUid.Value, args.WorldPoint);
			float volume = MathF.Min(10f, 1f * MathF.Pow(jungleDiff, 0.5f) - 5f);
			AudioParams audioParams = AudioParams.Default.WithVariation(new float?(0.05f)).WithVolume(volume);
			this._audio.Play(this._shuttleImpactSound, Filter.Pvs(coordinates, 4f, this.EntityManager, null), coordinates, true, new AudioParams?(audioParams));
		}

		// Token: 0x040005EC RID: 1516
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040005ED RID: 1517
		[Dependency]
		private readonly FixtureSystem _fixtures;

		// Token: 0x040005EE RID: 1518
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x040005EF RID: 1519
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040005F0 RID: 1520
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x040005F1 RID: 1521
		private ISawmill _sawmill;

		// Token: 0x040005F2 RID: 1522
		public const float TileMassMultiplier = 0.5f;

		// Token: 0x040005F3 RID: 1523
		public const float ShuttleLinearDamping = 0.05f;

		// Token: 0x040005F4 RID: 1524
		public const float ShuttleAngularDamping = 0.05f;

		// Token: 0x040005F5 RID: 1525
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040005F6 RID: 1526
		[Dependency]
		private readonly IdCardSystem _idSystem;

		// Token: 0x040005F7 RID: 1527
		[Dependency]
		private readonly AccessReaderSystem _reader;

		// Token: 0x040005F8 RID: 1528
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x040005F9 RID: 1529
		[Dependency]
		private readonly RoundEndSystem _roundEnd;

		// Token: 0x040005FC RID: 1532
		private float _consoleAccumulator;

		// Token: 0x040005FD RID: 1533
		private readonly TimeSpan _bufferTime = TimeSpan.FromSeconds(5.0);

		// Token: 0x040005FF RID: 1535
		private float _authorizeTime;

		// Token: 0x04000600 RID: 1536
		[Nullable(2)]
		private CancellationTokenSource _roundEndCancelToken;

		// Token: 0x04000601 RID: 1537
		private const string EmergencyRepealAllAccess = "EmergencyShuttleRepealAll";

		// Token: 0x04000602 RID: 1538
		private static readonly Color DangerColor = Color.Red;

		// Token: 0x04000603 RID: 1539
		private bool _launchedShuttles;

		// Token: 0x04000604 RID: 1540
		private bool _announced;

		// Token: 0x04000605 RID: 1541
		[Dependency]
		private readonly IAdminLogManager _logger;

		// Token: 0x04000606 RID: 1542
		[Dependency]
		private readonly IAdminManager _admin;

		// Token: 0x04000607 RID: 1543
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000608 RID: 1544
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000609 RID: 1545
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x0400060A RID: 1546
		[Dependency]
		private readonly CommunicationsConsoleSystem _commsConsole;

		// Token: 0x0400060B RID: 1547
		[Dependency]
		private readonly DockingSystem _dockSystem;

		// Token: 0x0400060C RID: 1548
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x0400060D RID: 1549
		[Dependency]
		private readonly StationSystem _station;

		// Token: 0x04000610 RID: 1552
		private float _shuttleIndex;

		// Token: 0x04000611 RID: 1553
		private const float ShuttleSpawnBuffer = 1f;

		// Token: 0x04000612 RID: 1554
		private bool _emergencyShuttleEnabled;

		// Token: 0x04000613 RID: 1555
		[Dependency]
		private readonly AirlockSystem _airlock;

		// Token: 0x04000614 RID: 1556
		[Dependency]
		private readonly DoorSystem _doors;

		// Token: 0x04000615 RID: 1557
		[Dependency]
		private readonly ShuttleConsoleSystem _console;

		// Token: 0x04000616 RID: 1558
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000617 RID: 1559
		[Dependency]
		private readonly StunSystem _stuns;

		// Token: 0x04000618 RID: 1560
		[Dependency]
		private readonly ThrusterSystem _thruster;

		// Token: 0x04000619 RID: 1561
		private MapId? _hyperSpaceMap;

		// Token: 0x0400061A RID: 1562
		private const float DefaultStartupTime = 5.5f;

		// Token: 0x0400061B RID: 1563
		private const float DefaultTravelTime = 30f;

		// Token: 0x0400061C RID: 1564
		private const float DefaultArrivalTime = 5f;

		// Token: 0x0400061D RID: 1565
		private const float FTLCooldown = 30f;

		// Token: 0x0400061E RID: 1566
		private const float ShuttleFTLRange = 100f;

		// Token: 0x0400061F RID: 1567
		private const float ShuttleFTLMassThreshold = 300f;

		// Token: 0x04000620 RID: 1568
		private readonly SoundSpecifier _startupSound = new SoundPathSpecifier("/Audio/Effects/Shuttle/hyperspace_begin.ogg", null);

		// Token: 0x04000621 RID: 1569
		private readonly SoundSpecifier _arrivalSound = new SoundPathSpecifier("/Audio/Effects/Shuttle/hyperspace_end.ogg", null);

		// Token: 0x04000622 RID: 1570
		private readonly TimeSpan _hyperspaceKnockdownTime = TimeSpan.FromSeconds(5.0);

		// Token: 0x04000623 RID: 1571
		private float _index;

		// Token: 0x04000624 RID: 1572
		private const float Buffer = 5f;

		// Token: 0x04000625 RID: 1573
		private const int FTLProximityIterations = 3;

		// Token: 0x04000626 RID: 1574
		private const int MinimumImpactVelocity = 10;

		// Token: 0x04000627 RID: 1575
		private readonly SoundCollectionSpecifier _shuttleImpactSound = new SoundCollectionSpecifier("ShuttleImpactSound", null);

		// Token: 0x0200090F RID: 2319
		[NullableContext(0)]
		private sealed class DockingConfig
		{
			// Token: 0x04001EAA RID: 7850
			[TupleElementNames(new string[]
			{
				"DockA",
				"DockB"
			})]
			[Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})]
			public List<ValueTuple<DockingComponent, DockingComponent>> Docks = new List<ValueTuple<DockingComponent, DockingComponent>>();

			// Token: 0x04001EAB RID: 7851
			public Box2 Area;

			// Token: 0x04001EAC RID: 7852
			public EntityUid TargetGrid;

			// Token: 0x04001EAD RID: 7853
			public EntityCoordinates Coordinates;

			// Token: 0x04001EAE RID: 7854
			public Angle Angle;
		}
	}
}
