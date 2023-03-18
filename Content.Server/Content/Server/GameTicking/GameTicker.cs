using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Announcements;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Database;
using Content.Server.GameTicking.Presets;
using Content.Server.GameTicking.Prototypes;
using Content.Server.GameTicking.Rules;
using Content.Server.Ghost;
using Content.Server.Ghost.Components;
using Content.Server.Maps;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Content.Server.Roles;
using Content.Server.ServerUpdates;
using Content.Server.Spawners.Components;
using Content.Server.Speech.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.White.JoinQueue;
using Content.Server.White.Stalin;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.GameTicking;
using Content.Shared.GameWindow;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Prometheus;
using Robust.Server;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Server.ServerStatus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Exceptions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A4 RID: 1188
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameTicker : SharedGameTicker
	{
		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060017DC RID: 6108 RVA: 0x0007C722 File Offset: 0x0007A922
		// (set) Token: 0x060017DD RID: 6109 RVA: 0x0007C72A File Offset: 0x0007A92A
		[ViewVariables]
		public MapId DefaultMap { get; private set; }

		// Token: 0x060017DE RID: 6110 RVA: 0x0007C734 File Offset: 0x0007A934
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = this._logManager.GetSawmill("ticker");
			this.InitializeStatusShell();
			this.InitializeCVars();
			this.InitializePlayer();
			this.InitializeLobbyMusic();
			this.InitializeLobbyBackground();
			this.InitializeGamePreset();
			this.InitializeGameRules();
			this._initialized = true;
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x0007C78E File Offset: 0x0007A98E
		public void PostInitialize()
		{
			this.RestartRound();
			this._postInitialized = true;
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x0007C79D File Offset: 0x0007A99D
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownGameRules();
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x0007C7AC File Offset: 0x0007A9AC
		private void SendServerMessage(string message)
		{
			this._chatManager.ChatMessageToAll(ChatChannel.Server, message, "", default(EntityUid), false, true, null, null, 0f);
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x0007C7E5 File Offset: 0x0007A9E5
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateRoundFlow(frameTime);
		}

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x060017E3 RID: 6115 RVA: 0x0007C7F5 File Offset: 0x0007A9F5
		// (set) Token: 0x060017E4 RID: 6116 RVA: 0x0007C7FD File Offset: 0x0007A9FD
		[ViewVariables]
		public bool LobbyEnabled { get; private set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x060017E5 RID: 6117 RVA: 0x0007C806 File Offset: 0x0007AA06
		// (set) Token: 0x060017E6 RID: 6118 RVA: 0x0007C80E File Offset: 0x0007AA0E
		[ViewVariables]
		public bool DummyTicker { get; private set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060017E7 RID: 6119 RVA: 0x0007C817 File Offset: 0x0007AA17
		// (set) Token: 0x060017E8 RID: 6120 RVA: 0x0007C81F File Offset: 0x0007AA1F
		[ViewVariables]
		public TimeSpan LobbyDuration { get; private set; } = TimeSpan.Zero;

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060017E9 RID: 6121 RVA: 0x0007C828 File Offset: 0x0007AA28
		// (set) Token: 0x060017EA RID: 6122 RVA: 0x0007C830 File Offset: 0x0007AA30
		[ViewVariables]
		public bool DisallowLateJoin { get; private set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060017EB RID: 6123 RVA: 0x0007C839 File Offset: 0x0007AA39
		// (set) Token: 0x060017EC RID: 6124 RVA: 0x0007C841 File Offset: 0x0007AA41
		[ViewVariables]
		public bool StationOffset { get; private set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060017ED RID: 6125 RVA: 0x0007C84A File Offset: 0x0007AA4A
		// (set) Token: 0x060017EE RID: 6126 RVA: 0x0007C852 File Offset: 0x0007AA52
		[ViewVariables]
		public bool StationRotation { get; private set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060017EF RID: 6127 RVA: 0x0007C85B File Offset: 0x0007AA5B
		// (set) Token: 0x060017F0 RID: 6128 RVA: 0x0007C863 File Offset: 0x0007AA63
		[ViewVariables]
		public float MaxStationOffset { get; private set; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060017F1 RID: 6129 RVA: 0x0007C86C File Offset: 0x0007AA6C
		// (set) Token: 0x060017F2 RID: 6130 RVA: 0x0007C874 File Offset: 0x0007AA74
		[ViewVariables]
		public int RoundStartFailShutdownCount { get; private set; }

		// Token: 0x060017F3 RID: 6131 RVA: 0x0007C880 File Offset: 0x0007AA80
		private void InitializeCVars()
		{
			this._configurationManager.OnValueChanged<bool>(CCVars.GameLobbyEnabled, delegate(bool value)
			{
				this.LobbyEnabled = value;
				foreach (KeyValuePair<NetUserId, PlayerGameStatus> keyValuePair in this._playerGameStatuses)
				{
					NetUserId netUserId;
					PlayerGameStatus playerGameStatus;
					keyValuePair.Deconstruct(out netUserId, out playerGameStatus);
					NetUserId userId = netUserId;
					if (playerGameStatus != PlayerGameStatus.JoinedGame)
					{
						this._playerGameStatuses[userId] = (this.LobbyEnabled ? PlayerGameStatus.NotReadyToPlay : PlayerGameStatus.ReadyToPlay);
					}
				}
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.GameDummyTicker, delegate(bool value)
			{
				this.DummyTicker = value;
			}, true);
			this._configurationManager.OnValueChanged<int>(CCVars.GameLobbyDuration, delegate(int value)
			{
				this.LobbyDuration = TimeSpan.FromSeconds((double)value);
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.GameDisallowLateJoins, delegate(bool value)
			{
				this.DisallowLateJoin = value;
				this.UpdateLateJoinStatus();
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.StationOffset, delegate(bool value)
			{
				this.StationOffset = value;
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.StationRotation, delegate(bool value)
			{
				this.StationRotation = value;
			}, true);
			this._configurationManager.OnValueChanged<float>(CCVars.MaxStationOffset, delegate(float value)
			{
				this.MaxStationOffset = value;
			}, true);
			this._configurationManager.OnValueChanged<int>(CCVars.RoundStartFailShutdownCount, delegate(int value)
			{
				this.RoundStartFailShutdownCount = value;
			}, true);
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x0007C975 File Offset: 0x0007AB75
		// (set) Token: 0x060017F5 RID: 6133 RVA: 0x0007C97D File Offset: 0x0007AB7D
		[Nullable(2)]
		public GamePresetPrototype Preset { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x060017F6 RID: 6134 RVA: 0x0007C988 File Offset: 0x0007AB88
		private bool StartPreset(IPlayerSession[] origReadyPlayers, bool force)
		{
			GameTicker.<>c__DisplayClass79_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			RoundStartAttemptEvent startAttempt = new RoundStartAttemptEvent(origReadyPlayers, force);
			base.RaiseLocalEvent<RoundStartAttemptEvent>(startAttempt);
			if (!startAttempt.Cancelled)
			{
				return true;
			}
			CS$<>8__locals1.presetTitle = ((this.Preset != null) ? Loc.GetString(this.Preset.ModeTitle) : string.Empty);
			if (!this._configurationManager.GetCVar<bool>(CCVars.GameLobbyFallbackEnabled))
			{
				this.<StartPreset>g__FailedPresetRestart|79_0(ref CS$<>8__locals1);
				return false;
			}
			GamePresetPrototype preset = this.Preset;
			this.ClearGameRules();
			this.SetGamePreset(this._configurationManager.GetCVar<string>(CCVars.GameLobbyFallbackPreset), false);
			this.AddGamePresetRules();
			this.StartGamePresetRules();
			startAttempt.Uncancel();
			base.RaiseLocalEvent<RoundStartAttemptEvent>(startAttempt);
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("game-ticker-start-round-cannot-start-game-mode-fallback", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("failedGameMode", CS$<>8__locals1.presetTitle),
				new ValueTuple<string, object>("fallbackMode", Loc.GetString(this.Preset.ModeTitle))
			}), null);
			if (startAttempt.Cancelled)
			{
				this.<StartPreset>g__FailedPresetRestart|79_0(ref CS$<>8__locals1);
				return false;
			}
			this.RefreshLateJoinAllowed();
			return true;
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x0007CAB0 File Offset: 0x0007ACB0
		private void InitializeGamePreset()
		{
			this.SetGamePreset(this.LobbyEnabled ? this._configurationManager.GetCVar<string>(CCVars.GameLobbyDefaultPreset) : "sandbox", false);
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x0007CAD8 File Offset: 0x0007ACD8
		public void SetGamePreset(GamePresetPrototype preset, bool force = false)
		{
			if (this.DummyTicker)
			{
				return;
			}
			this.Preset = preset;
			this.UpdateInfoText();
			if (force)
			{
				this.StartRound(true);
			}
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0007CAFC File Offset: 0x0007ACFC
		public void SetGamePreset(string preset, bool force = false)
		{
			GamePresetPrototype proto = this.FindGamePreset(preset);
			if (proto != null)
			{
				this.SetGamePreset(proto, force);
			}
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0007CB1C File Offset: 0x0007AD1C
		[return: Nullable(2)]
		public GamePresetPrototype FindGamePreset(string preset)
		{
			GamePresetPrototype presetProto;
			if (this._prototypeManager.TryIndex<GamePresetPrototype>(preset, ref presetProto))
			{
				return presetProto;
			}
			foreach (GamePresetPrototype proto in this._prototypeManager.EnumeratePrototypes<GamePresetPrototype>())
			{
				foreach (string alias in proto.Alias)
				{
					if (preset.Equals(alias, StringComparison.InvariantCultureIgnoreCase))
					{
						return proto;
					}
				}
			}
			return null;
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x0007CBAC File Offset: 0x0007ADAC
		public bool TryFindGamePreset(string preset, [Nullable(2)] [NotNullWhen(true)] out GamePresetPrototype prototype)
		{
			prototype = this.FindGamePreset(preset);
			return prototype != null;
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x0007CBBC File Offset: 0x0007ADBC
		private bool AddGamePresetRules()
		{
			if (this.DummyTicker || this.Preset == null)
			{
				return false;
			}
			foreach (string rule in this.Preset.Rules)
			{
				GameRulePrototype ruleProto;
				if (this._prototypeManager.TryIndex<GameRulePrototype>(rule, ref ruleProto))
				{
					this.AddGameRule(ruleProto);
				}
			}
			return true;
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x0007CC34 File Offset: 0x0007AE34
		private void StartGamePresetRules()
		{
			foreach (GameRulePrototype rule in this._addedGameRules.ToArray<GameRulePrototype>())
			{
				this.StartGameRule(rule);
			}
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0007CC68 File Offset: 0x0007AE68
		public bool OnGhostAttempt(Mind mind, bool canReturnGlobal, bool viaCommand = false)
		{
			EntityUid? playerEntity = mind.CurrentEntity;
			if (playerEntity != null && viaCommand)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Mind;
				LogStringHandler logStringHandler = new LogStringHandler(35, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(playerEntity.Value), "player", "EntityManager.ToPrettyString(playerEntity.Value)");
				logStringHandler.AppendLiteral(" is attempting to ghost via command");
				adminLogger.Add(type, ref logStringHandler);
			}
			GhostAttemptHandleEvent handleEv = new GhostAttemptHandleEvent(mind, canReturnGlobal);
			base.RaiseLocalEvent<GhostAttemptHandleEvent>(handleEv);
			if (handleEv.Handled)
			{
				return handleEv.Result;
			}
			if (mind.PreventGhosting)
			{
				if (mind.Session != null)
				{
					this._chatManager.DispatchServerMessage(mind.Session, Loc.GetString("comp-mind-ghosting-prevented"), true);
				}
				return false;
			}
			if (base.HasComp<GhostComponent>(playerEntity))
			{
				return false;
			}
			if (mind.VisitingEntity != null)
			{
				mind.UnVisit();
			}
			EntityCoordinates position = (playerEntity != null && playerEntity.GetValueOrDefault().Valid) ? base.Transform(playerEntity.Value).Coordinates : this.GetObserverSpawnPoint();
			if (position == default(EntityCoordinates))
			{
				return false;
			}
			bool canReturn = canReturnGlobal && mind.CharacterDeadPhysically;
			MobStateComponent mobState;
			if (canReturnGlobal && base.TryComp<MobStateComponent>(playerEntity, ref mobState) && this._mobStateSystem.IsCritical(playerEntity.Value, mobState))
			{
				canReturn = true;
				DamageSpecifier damage = new DamageSpecifier(this._prototypeManager.Index<DamageTypePrototype>("Asphyxiation"), 200);
				this._damageable.TryChangeDamage(playerEntity, damage, true, true, null, null);
			}
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityCoordinates coords = this._transform.GetMoverCoordinates(position, xformQuery);
			EntityUid ghost = base.Spawn("MobObserver", coords);
			MetaDataComponent meta = base.MetaData(ghost);
			if (!string.IsNullOrWhiteSpace(mind.CharacterName))
			{
				meta.EntityName = mind.CharacterName;
			}
			else
			{
				IPlayerSession session = mind.Session;
				if (!string.IsNullOrWhiteSpace((session != null) ? session.Name : null))
				{
					meta.EntityName = mind.Session.Name;
				}
			}
			GhostComponent ghostComponent = base.Comp<GhostComponent>(ghost);
			if (mind.TimeOfDeath != null)
			{
				ghostComponent.TimeOfDeath = mind.TimeOfDeath.Value;
			}
			if (playerEntity != null)
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Mind;
				LogStringHandler logStringHandler = new LogStringHandler(8, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(playerEntity.Value), "player", "EntityManager.ToPrettyString(playerEntity.Value)");
				logStringHandler.AppendLiteral(" ghosted");
				logStringHandler.AppendFormatted((!canReturn) ? " (non-returnable)" : "");
				adminLogger2.Add(type2, ref logStringHandler);
			}
			this._ghosts.SetCanReturnToBody(ghostComponent, canReturn);
			if (canReturn)
			{
				mind.Visit(ghost);
			}
			else
			{
				mind.TransferTo(new EntityUid?(ghost), false, false);
			}
			NetUserId userId = mind.Session.UserId;
			TimeSpan timeSpan;
			if (!this._ghostSystem._deathTime.TryGetValue(userId, out timeSpan))
			{
				this._ghostSystem._deathTime[userId] = this._gameTiming.CurTime;
			}
			return true;
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0007CF74 File Offset: 0x0007B174
		private void IncrementRoundNumber()
		{
			GameTicker.<>c__DisplayClass88_0 CS$<>8__locals1 = new GameTicker.<>c__DisplayClass88_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.playerIds = (from player in this._playerGameStatuses.Keys
			select player.UserId).ToArray<Guid>();
			CS$<>8__locals1.serverName = this._configurationManager.GetCVar<string>(CCVars.AdminLogsServerName);
			Task<int> task = Task.Run<int>(delegate()
			{
				GameTicker.<>c__DisplayClass88_0.<<IncrementRoundNumber>b__1>d <<IncrementRoundNumber>b__1>d;
				<<IncrementRoundNumber>b__1>d.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
				<<IncrementRoundNumber>b__1>d.<>4__this = CS$<>8__locals1;
				<<IncrementRoundNumber>b__1>d.<>1__state = -1;
				<<IncrementRoundNumber>b__1>d.<>t__builder.Start<GameTicker.<>c__DisplayClass88_0.<<IncrementRoundNumber>b__1>d>(ref <<IncrementRoundNumber>b__1>d);
				return <<IncrementRoundNumber>b__1>d.<>t__builder.Task;
			});
			this._taskManager.BlockWaitOnTask(task);
			this.RoundId = task.GetAwaiter().GetResult();
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06001800 RID: 6144 RVA: 0x0007D00E File Offset: 0x0007B20E
		public IReadOnlySet<GameRulePrototype> AddedGameRules
		{
			get
			{
				return this._addedGameRules;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06001801 RID: 6145 RVA: 0x0007D016 File Offset: 0x0007B216
		public IReadOnlySet<GameRulePrototype> StartedGameRules
		{
			get
			{
				return this._startedGameRules;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06001802 RID: 6146 RVA: 0x0007D01E File Offset: 0x0007B21E
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public IReadOnlyList<ValueTuple<TimeSpan, GameRulePrototype>> AllPreviousGameRules
		{
			[return: Nullable(new byte[]
			{
				1,
				0,
				1
			})]
			get
			{
				return this._allPreviousGameRules;
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x0007D028 File Offset: 0x0007B228
		private void InitializeGameRules()
		{
			this._consoleHost.RegisterCommand("addgamerule", string.Empty, "addgamerule <rules>", new ConCommandCallback(this.AddGameRuleCommand), new ConCommandCompletionCallback(this.AddGameRuleCompletions), false);
			this._consoleHost.RegisterCommand("endgamerule", string.Empty, "endgamerule <rules>", new ConCommandCallback(this.EndGameRuleCommand), new ConCommandCompletionCallback(this.EndGameRuleCompletions), false);
			this._consoleHost.RegisterCommand("cleargamerules", string.Empty, "cleargamerules", new ConCommandCallback(this.ClearGameRulesCommand), false);
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x0007D0C2 File Offset: 0x0007B2C2
		private void ShutdownGameRules()
		{
			this._consoleHost.UnregisterCommand("addgamerule");
			this._consoleHost.UnregisterCommand("endgamerule");
			this._consoleHost.UnregisterCommand("cleargamerules");
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x0007D0F4 File Offset: 0x0007B2F4
		public void StartGameRule(GameRulePrototype rule)
		{
			if (!this.IsGameRuleAdded(rule))
			{
				this.AddGameRule(rule);
			}
			this._allPreviousGameRules.Add(new ValueTuple<TimeSpan, GameRulePrototype>(this.RoundDuration(), rule));
			this._sawmill.Info("Started game rule " + rule.ID);
			if (this._startedGameRules.Add(rule))
			{
				base.RaiseLocalEvent<GameRuleStartedEvent>(new GameRuleStartedEvent(rule));
			}
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0007D160 File Offset: 0x0007B360
		public void EndGameRule(GameRulePrototype rule)
		{
			if (!this.IsGameRuleAdded(rule))
			{
				return;
			}
			this._addedGameRules.Remove(rule);
			this._sawmill.Info("Ended game rule " + rule.ID);
			if (this.IsGameRuleStarted(rule))
			{
				this._startedGameRules.Remove(rule);
			}
			base.RaiseLocalEvent<GameRuleEndedEvent>(new GameRuleEndedEvent(rule));
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x0007D1C1 File Offset: 0x0007B3C1
		public bool AddGameRule(GameRulePrototype rule)
		{
			if (!this._addedGameRules.Add(rule))
			{
				return false;
			}
			this._sawmill.Info("Added game rule " + rule.ID);
			base.RaiseLocalEvent<GameRuleAddedEvent>(new GameRuleAddedEvent(rule));
			return true;
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0007D1FB File Offset: 0x0007B3FB
		public bool IsGameRuleAdded(GameRulePrototype rule)
		{
			return this._addedGameRules.Contains(rule);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007D20C File Offset: 0x0007B40C
		public bool IsGameRuleAdded(string rule)
		{
			using (HashSet<GameRulePrototype>.Enumerator enumerator = this._addedGameRules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ID.Equals(rule))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x0007D26C File Offset: 0x0007B46C
		public bool IsGameRuleStarted(GameRulePrototype rule)
		{
			return this._startedGameRules.Contains(rule);
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x0007D27C File Offset: 0x0007B47C
		public bool IsGameRuleStarted(string rule)
		{
			using (HashSet<GameRulePrototype>.Enumerator enumerator = this._startedGameRules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ID.Equals(rule))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x0007D2DC File Offset: 0x0007B4DC
		public void ClearGameRules()
		{
			foreach (GameRulePrototype rule in this._addedGameRules.ToArray<GameRulePrototype>())
			{
				this.EndGameRule(rule);
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x0007D310 File Offset: 0x0007B510
		[AdminCommand(AdminFlags.Fun)]
		private void AddGameRuleCommand(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length == 0)
			{
				return;
			}
			foreach (string ruleId in args)
			{
				GameRulePrototype rule;
				if (this._prototypeManager.TryIndex<GameRulePrototype>(ruleId, ref rule))
				{
					this.AddGameRule(rule);
					if (this.RunLevel == GameRunLevel.InRound)
					{
						this.StartGameRule(rule);
					}
				}
			}
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x0007D360 File Offset: 0x0007B560
		private CompletionResult AddGameRuleCompletions(IConsoleShell shell, string[] args)
		{
			IEnumerable<string> activeIds = from c in this._addedGameRules
			select c.ID;
			return CompletionResult.FromHintOptions(from p in CompletionHelper.PrototypeIDs<GameRulePrototype>(true, null)
			where !activeIds.Contains(p.Value)
			select p, "<rule>");
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x0007D3C8 File Offset: 0x0007B5C8
		[AdminCommand(AdminFlags.Fun)]
		private void EndGameRuleCommand(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length == 0)
			{
				return;
			}
			foreach (string ruleId in args)
			{
				GameRulePrototype rule;
				if (this._prototypeManager.TryIndex<GameRulePrototype>(ruleId, ref rule))
				{
					this.EndGameRule(rule);
				}
			}
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0007D405 File Offset: 0x0007B605
		private CompletionResult EndGameRuleCompletions(IConsoleShell shell, string[] args)
		{
			return CompletionResult.FromHintOptions(from c in this._addedGameRules
			select new CompletionOption(c.ID, null, 0), "<added rule>");
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x0007D43B File Offset: 0x0007B63B
		[AdminCommand(AdminFlags.Fun)]
		private void ClearGameRulesCommand(IConsoleShell shell, string argstr, string[] args)
		{
			this.ClearGameRules();
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06001812 RID: 6162 RVA: 0x0007D443 File Offset: 0x0007B643
		// (set) Token: 0x06001813 RID: 6163 RVA: 0x0007D44B File Offset: 0x0007B64B
		[ViewVariables]
		public bool Paused { get; set; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06001814 RID: 6164 RVA: 0x0007D454 File Offset: 0x0007B654
		public IReadOnlyDictionary<NetUserId, PlayerGameStatus> PlayerGameStatuses
		{
			get
			{
				return this._playerGameStatuses;
			}
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x0007D45C File Offset: 0x0007B65C
		public void UpdateInfoText()
		{
			base.RaiseNetworkEvent(this.GetInfoMsg(), Filter.Empty().AddPlayers(this._playerManager.NetworkedSessions), true);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x0007D480 File Offset: 0x0007B680
		private string GetInfoText()
		{
			if (this.Preset == null)
			{
				return string.Empty;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<int>(this._playerManager.PlayerCount);
			string playerCount = defaultInterpolatedStringHandler.ToStringAndClear();
			int readyCount = this._playerGameStatuses.Values.Count((PlayerGameStatus x) => x == PlayerGameStatus.ReadyToPlay);
			StringBuilder stationNames = new StringBuilder();
			if (this._stationSystem.Stations.Count != 0)
			{
				using (IEnumerator<EntityUid> enumerator = this._stationSystem.Stations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						EntityUid entUID = enumerator.Current;
						StationDataComponent stationData = null;
						MetaDataComponent metaData = null;
						if (base.Resolve<StationDataComponent, MetaDataComponent>(entUID, ref stationData, ref metaData, true))
						{
							if (stationNames.Length > 0)
							{
								stationNames.Append('\n');
							}
							stationNames.Append(metaData.EntityName);
						}
					}
					goto IL_F3;
				}
			}
			stationNames.Append(Loc.GetString("game-ticker-no-map-selected"));
			IL_F3:
			string gmTitle = Loc.GetString(this.Preset.ModeTitle);
			string desc = Loc.GetString(this.Preset.Description);
			return Loc.GetString((this.RunLevel == GameRunLevel.PreRoundLobby) ? "game-ticker-get-info-preround-text" : "game-ticker-get-info-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("roundId", this.RoundId),
				new ValueTuple<string, object>("playerCount", playerCount),
				new ValueTuple<string, object>("readyCount", readyCount),
				new ValueTuple<string, object>("mapName", stationNames.ToString()),
				new ValueTuple<string, object>("gmTitle", gmTitle),
				new ValueTuple<string, object>("desc", desc)
			});
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x0007D654 File Offset: 0x0007B854
		private TickerLobbyReadyEvent GetStatusSingle(ICommonSession player, PlayerGameStatus gameStatus)
		{
			return new TickerLobbyReadyEvent(new Dictionary<NetUserId, PlayerGameStatus>
			{
				{
					player.UserId,
					gameStatus
				}
			});
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x0007D670 File Offset: 0x0007B870
		private TickerLobbyReadyEvent GetPlayerStatus()
		{
			Dictionary<NetUserId, PlayerGameStatus> players = new Dictionary<NetUserId, PlayerGameStatus>();
			foreach (NetUserId player in this._playerGameStatuses.Keys)
			{
				PlayerGameStatus status;
				this._playerGameStatuses.TryGetValue(player, out status);
				players.Add(player, status);
			}
			return new TickerLobbyReadyEvent(players);
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0007D6E4 File Offset: 0x0007B8E4
		private TickerLobbyStatusEvent GetStatusMsg(IPlayerSession session)
		{
			PlayerGameStatus status;
			this._playerGameStatuses.TryGetValue(session.UserId, out status);
			return new TickerLobbyStatusEvent(this.RunLevel > GameRunLevel.PreRoundLobby, this.LobbySong, this.LobbyBackground, status == PlayerGameStatus.ReadyToPlay, this._roundStartTime, this._roundStartTimeSpan, this.Paused);
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0007D738 File Offset: 0x0007B938
		private void SendStatusToAll()
		{
			foreach (IPlayerSession player in this._playerManager.ServerSessions)
			{
				base.RaiseNetworkEvent(this.GetStatusMsg(player), player.ConnectedClient);
			}
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x0007D798 File Offset: 0x0007B998
		private TickerLobbyInfoEvent GetInfoMsg()
		{
			return new TickerLobbyInfoEvent(this.GetInfoText());
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x0007D7A5 File Offset: 0x0007B9A5
		private void UpdateLateJoinStatus()
		{
			base.RaiseNetworkEvent(new TickerLateJoinStatusEvent(this.DisallowLateJoin));
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0007D7B8 File Offset: 0x0007B9B8
		public bool PauseStart(bool pause = true)
		{
			if (this.Paused == pause)
			{
				return false;
			}
			this.Paused = pause;
			if (pause)
			{
				this._pauseTime = this._gameTiming.CurTime;
			}
			else if (this._pauseTime != default(TimeSpan))
			{
				this._roundStartTime += this._gameTiming.CurTime - this._pauseTime;
			}
			base.RaiseNetworkEvent(new TickerLobbyCountdownEvent(this._roundStartTime, this.Paused));
			this._chatManager.DispatchServerAnnouncement(Loc.GetString(this.Paused ? "game-ticker-pause-start" : "game-ticker-pause-start-resumed"), null);
			return true;
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x0007D86F File Offset: 0x0007BA6F
		public bool TogglePause()
		{
			this.PauseStart(!this.Paused);
			return this.Paused;
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x0007D888 File Offset: 0x0007BA88
		public void ToggleReadyAll(bool ready)
		{
			PlayerGameStatus status = ready ? PlayerGameStatus.ReadyToPlay : PlayerGameStatus.NotReadyToPlay;
			foreach (NetUserId playerUserId in this._playerGameStatuses.Keys)
			{
				this._playerGameStatuses[playerUserId] = status;
				IPlayerSession playerSession;
				if (this._playerManager.TryGetSessionById(playerUserId, ref playerSession))
				{
					base.RaiseNetworkEvent(this.GetStatusMsg(playerSession), playerSession.ConnectedClient);
					base.RaiseNetworkEvent(this.GetStatusSingle(playerSession, status));
				}
			}
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x0007D920 File Offset: 0x0007BB20
		public void ToggleReady(IPlayerSession player, bool ready)
		{
			if (!this._playerGameStatuses.ContainsKey(player.UserId))
			{
				return;
			}
			if (!this._userDb.IsLoadComplete(player))
			{
				return;
			}
			if (this._configurationManager.GetCVar<bool>(CCVars.StalinEnabled))
			{
				this._chatManager.DispatchServerMessage(player, "Внимание, на сервере включен бункер. Если ваш аккаунт не был привязан к дискорду, то вы не сможете зайти в раунд. Для того чтобы привязать аккаунт - нажмите на кнопку ПРИВЯЗАТЬ АККАУНТ", false);
			}
			PlayerGameStatus status = ready ? PlayerGameStatus.ReadyToPlay : PlayerGameStatus.NotReadyToPlay;
			this._playerGameStatuses[player.UserId] = (ready ? PlayerGameStatus.ReadyToPlay : PlayerGameStatus.NotReadyToPlay);
			base.RaiseNetworkEvent(this.GetStatusMsg(player), player.ConnectedClient);
			base.RaiseNetworkEvent(this.GetStatusSingle(player, status));
			this.UpdateInfoText();
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06001821 RID: 6177 RVA: 0x0007D9BB File Offset: 0x0007BBBB
		// (set) Token: 0x06001822 RID: 6178 RVA: 0x0007D9C3 File Offset: 0x0007BBC3
		[Nullable(2)]
		[ViewVariables]
		public string LobbyBackground { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x06001823 RID: 6179 RVA: 0x0007D9CC File Offset: 0x0007BBCC
		private void InitializeLobbyBackground()
		{
			this._lobbyBackgrounds = (from x in this._prototypeManager.EnumeratePrototypes<LobbyBackgroundPrototype>()
			select x.Background into x
			where GameTicker.WhitelistedBackgroundExtensions.Contains(x.Extension)
			select x).ToList<ResourcePath>();
			this.RandomizeLobbyBackground();
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x0007DA3D File Offset: 0x0007BC3D
		private void RandomizeLobbyBackground()
		{
			this.LobbyBackground = (this._lobbyBackgrounds.Any<ResourcePath>() ? RandomExtensions.Pick<ResourcePath>(this._robustRandom, this._lobbyBackgrounds).ToString() : null);
		}

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06001825 RID: 6181 RVA: 0x0007DA6B File Offset: 0x0007BC6B
		// (set) Token: 0x06001826 RID: 6182 RVA: 0x0007DA73 File Offset: 0x0007BC73
		[Nullable(2)]
		[ViewVariables]
		public string LobbySong { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x06001827 RID: 6183 RVA: 0x0007DA7C File Offset: 0x0007BC7C
		private void InitializeLobbyMusic()
		{
			this._lobbyMusicCollection = this._prototypeManager.Index<SoundCollectionPrototype>("LobbyMusic");
			this._lobbyMusicInitialized = true;
			this.ChooseRandomLobbySong();
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0007DAA1 File Offset: 0x0007BCA1
		[NullableContext(2)]
		public void SetLobbySong(string song)
		{
			if (song == null)
			{
				this.LobbySong = null;
				return;
			}
			this.LobbySong = song;
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0007DAB5 File Offset: 0x0007BCB5
		public void ChooseRandomLobbySong()
		{
			this.SetLobbySong(RandomExtensions.Pick<ResourcePath>(this._robustRandom, this._lobbyMusicCollection.PickFiles).ToString());
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x0007DAD8 File Offset: 0x0007BCD8
		public void StopLobbySong()
		{
			this.SetLobbySong(null);
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x0007DAE1 File Offset: 0x0007BCE1
		private void InitializePlayer()
		{
			this._playerManager.PlayerStatusChanged += this.PlayerStatusChanged;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x0007DAFC File Offset: 0x0007BCFC
		private void PlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs args)
		{
			GameTicker.<PlayerStatusChanged>d__157 <PlayerStatusChanged>d__;
			<PlayerStatusChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PlayerStatusChanged>d__.<>4__this = this;
			<PlayerStatusChanged>d__.args = args;
			<PlayerStatusChanged>d__.<>1__state = -1;
			<PlayerStatusChanged>d__.<>t__builder.Start<GameTicker.<PlayerStatusChanged>d__157>(ref <PlayerStatusChanged>d__);
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007DB3B File Offset: 0x0007BD3B
		private HumanoidCharacterProfile GetPlayerProfile(IPlayerSession p)
		{
			return (HumanoidCharacterProfile)this._prefsManager.GetPreferences(p.UserId).SelectedCharacter;
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x0007DB58 File Offset: 0x0007BD58
		public void PlayerJoinGame(IPlayerSession session)
		{
			this._chatManager.DispatchServerMessage(session, Loc.GetString("game-ticker-player-join-game-message"), false);
			this._playerGameStatuses[session.UserId] = PlayerGameStatus.JoinedGame;
			this._db.AddRoundPlayers(this.RoundId, new Guid[]
			{
				session.UserId
			});
			base.RaiseNetworkEvent(new TickerJoinGameEvent(), session.ConnectedClient);
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0007DBCC File Offset: 0x0007BDCC
		private void PlayerJoinLobby(IPlayerSession session)
		{
			this._playerGameStatuses[session.UserId] = (this.LobbyEnabled ? PlayerGameStatus.NotReadyToPlay : PlayerGameStatus.ReadyToPlay);
			this._db.AddRoundPlayers(this.RoundId, new Guid[]
			{
				session.UserId
			});
			INetChannel client = session.ConnectedClient;
			base.RaiseNetworkEvent(new TickerJoinLobbyEvent(), client);
			base.RaiseNetworkEvent(this.GetStatusMsg(session), client);
			base.RaiseNetworkEvent(this.GetInfoMsg(), client);
			base.RaiseNetworkEvent(this.GetPlayerStatus(), client);
			base.RaiseLocalEvent<PlayerJoinedLobbyEvent>(new PlayerJoinedLobbyEvent(session));
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x0007DC67 File Offset: 0x0007BE67
		private void ReqWindowAttentionAll()
		{
			base.RaiseNetworkEvent(new RequestWindowAttentionEvent());
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x0007DC74 File Offset: 0x0007BE74
		// (set) Token: 0x06001832 RID: 6194 RVA: 0x0007DC7C File Offset: 0x0007BE7C
		[ViewVariables]
		public GameRunLevel RunLevel
		{
			get
			{
				return this._runLevel;
			}
			private set
			{
				GameRunLevel old = this._runLevel;
				this._runLevel = value;
				base.RaiseLocalEvent<GameRunLevelChangedEvent>(new GameRunLevelChangedEvent(old, value));
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x0007DCA4 File Offset: 0x0007BEA4
		// (set) Token: 0x06001834 RID: 6196 RVA: 0x0007DCAC File Offset: 0x0007BEAC
		[ViewVariables]
		public int RoundId { get; private set; }

		// Token: 0x06001835 RID: 6197 RVA: 0x0007DCB8 File Offset: 0x0007BEB8
		private void LoadMaps()
		{
			this.AddGamePresetRules();
			this.DefaultMap = this._mapManager.CreateMap(null);
			this._mapManager.AddUninitializedMap(this.DefaultMap);
			TimeSpan startTime = this._gameTiming.RealTime;
			List<GameMapPrototype> maps = new List<GameMapPrototype>();
			GameMapPrototype mainStationMap = this._gameMapManager.GetSelectedMap();
			if (mainStationMap == null)
			{
				this._gameMapManager.SelectMapByConfigRules();
				mainStationMap = this._gameMapManager.GetSelectedMap();
			}
			if (mainStationMap != null)
			{
				maps.Add(mainStationMap);
				base.RaiseLocalEvent<LoadingMapsEvent>(new LoadingMapsEvent(maps));
				foreach (GameMapPrototype map in maps)
				{
					MapId toLoad = this.DefaultMap;
					if (maps[0] != map)
					{
						toLoad = this._mapManager.CreateMap(null);
						this._mapManager.AddUninitializedMap(toLoad);
					}
					this.LoadGameMap(map, toLoad, null, null);
				}
				TimeSpan timeSpan = this._gameTiming.RealTime - startTime;
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Loaded maps in ");
				defaultInterpolatedStringHandler.AppendFormatted<double>(timeSpan.TotalMilliseconds, "N2");
				defaultInterpolatedStringHandler.AppendLiteral("ms.");
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			throw new Exception("invalid config; couldn't select a valid station map!");
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x0007DE2C File Offset: 0x0007C02C
		public IReadOnlyList<EntityUid> LoadGameMap(GameMapPrototype map, MapId targetMapId, [Nullable(2)] MapLoadOptions loadOptions, [Nullable(2)] string stationName = null)
		{
			MapLoadOptions loadOpts = loadOptions ?? new MapLoadOptions();
			PreGameMapLoad ev = new PreGameMapLoad(targetMapId, map, loadOpts);
			base.RaiseLocalEvent<PreGameMapLoad>(ev);
			List<EntityUid> gridUids = this._map.LoadMap(targetMapId, ev.GameMap.MapPath.ToString(), ev.Options).ToList<EntityUid>();
			base.RaiseLocalEvent<PostGameMapLoad>(new PostGameMapLoad(map, targetMapId, gridUids, stationName));
			return gridUids;
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0007DE90 File Offset: 0x0007C090
		public void StartRound(bool force = false)
		{
			GameTicker.<StartRound>d__179 <StartRound>d__;
			<StartRound>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartRound>d__.<>4__this = this;
			<StartRound>d__.force = force;
			<StartRound>d__.<>1__state = -1;
			<StartRound>d__.<>t__builder.Start<GameTicker.<StartRound>d__179>(ref <StartRound>d__);
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0007DED0 File Offset: 0x0007C0D0
		private void RefreshLateJoinAllowed()
		{
			RefreshLateJoinAllowedEvent refresh = new RefreshLateJoinAllowedEvent();
			base.RaiseLocalEvent<RefreshLateJoinAllowedEvent>(refresh);
			this.DisallowLateJoin = refresh.DisallowLateJoin;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x0007DEF6 File Offset: 0x0007C0F6
		public void EndRound(string text = "")
		{
			if (this.DummyTicker)
			{
				return;
			}
			this._sawmill.Info("Ending round!");
			this.RunLevel = GameRunLevel.PostRound;
			this.ShowRoundEndScoreboard(text);
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x0007DF20 File Offset: 0x0007C120
		public void ShowRoundEndScoreboard(string text = "")
		{
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.EmergencyShuttle;
			LogImpact impact = LogImpact.High;
			LogStringHandler logStringHandler = new LogStringHandler(28, 0);
			logStringHandler.AppendLiteral("Round ended, showing summary");
			adminLogger.Add(type, impact, ref logStringHandler);
			string gamemodeTitle = (this.Preset != null) ? Loc.GetString(this.Preset.ModeTitle) : string.Empty;
			RoundEndTextAppendEvent textEv = new RoundEndTextAppendEvent();
			base.RaiseLocalEvent<RoundEndTextAppendEvent>(textEv);
			string roundEndText = text + "\n" + textEv.Text;
			TimeSpan roundDuration = this.RoundDuration();
			List<RoundEndMessageEvent.RoundEndPlayerInfo> listOfPlayerInfo = new List<RoundEndMessageEvent.RoundEndPlayerInfo>();
			foreach (Mind mind in EntitySystem.Get<MindTrackerSystem>().AllMinds)
			{
				if (mind != null)
				{
					NetUserId userId = mind.OriginalOwnerUserId;
					userId.ToString();
					bool connected = false;
					bool observer = mind.AllRoles.Any((Role role) => role is ObserverRole);
					IPlayerSession ply;
					if (this._playerManager.TryGetSessionById(userId, ref ply))
					{
						connected = true;
					}
					PlayerData contentPlayerData = null;
					IPlayerData playerData;
					if (this._playerManager.TryGetPlayerData(userId, ref playerData))
					{
						contentPlayerData = playerData.ContentData();
					}
					bool antag = mind.AllRoles.Any((Role role) => role.Antagonist);
					string playerIcName = "Unknown";
					string icName;
					if (mind.CharacterName != null)
					{
						playerIcName = mind.CharacterName;
					}
					else if (mind.CurrentEntity != null && base.TryName(mind.CurrentEntity.Value, ref icName, null))
					{
						playerIcName = icName;
					}
					RoundEndMessageEvent.RoundEndPlayerInfo roundEndPlayerInfo = default(RoundEndMessageEvent.RoundEndPlayerInfo);
					roundEndPlayerInfo.PlayerOOCName = (((contentPlayerData != null) ? contentPlayerData.Name : null) ?? "(IMPOSSIBLE: REGISTERED MIND WITH NO OWNER)");
					roundEndPlayerInfo.PlayerICName = playerIcName;
					roundEndPlayerInfo.PlayerEntityUid = mind.OwnedEntity;
					string role2;
					if (!antag)
					{
						Role role3 = mind.AllRoles.FirstOrDefault<Role>();
						role2 = (((role3 != null) ? role3.Name : null) ?? Loc.GetString("game-ticker-unknown-role"));
					}
					else
					{
						role2 = mind.AllRoles.First((Role role) => role.Antagonist).Name;
					}
					roundEndPlayerInfo.Role = role2;
					roundEndPlayerInfo.Antag = antag;
					roundEndPlayerInfo.Observer = observer;
					roundEndPlayerInfo.Connected = connected;
					RoundEndMessageEvent.RoundEndPlayerInfo playerEndRoundInfo = roundEndPlayerInfo;
					listOfPlayerInfo.Add(playerEndRoundInfo);
				}
			}
			RoundEndMessageEvent.RoundEndPlayerInfo[] listOfPlayerInfoFinal = (from pi in listOfPlayerInfo
			orderby pi.PlayerOOCName
			select pi).ToArray<RoundEndMessageEvent.RoundEndPlayerInfo>();
			base.RaiseNetworkEvent(new RoundEndMessageEvent(gamemodeTitle, roundEndText, roundDuration, this.RoundId, listOfPlayerInfoFinal.Length, listOfPlayerInfoFinal, this.LobbySong, new SoundCollectionSpecifier("RoundEnd", null).GetSound(null, null)));
			base.RaiseLocalEvent<RoundEndedEvent>(new RoundEndedEvent(this.RoundId, roundDuration));
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x0007E234 File Offset: 0x0007C434
		public void RestartRound()
		{
			if (this.DummyTicker)
			{
				return;
			}
			if (this._serverUpdates.RoundEnded())
			{
				return;
			}
			this._sawmill.Info("Restarting round!");
			this.SendServerMessage(Loc.GetString("game-ticker-restart-round"));
			GameTicker.RoundNumberMetric.Inc(1.0);
			this.PlayersJoinedRoundNormally = 0;
			this.RunLevel = GameRunLevel.PreRoundLobby;
			this.LobbySong = RandomExtensions.Pick<ResourcePath>(this._robustRandom, this._lobbyMusicCollection.PickFiles).ToString();
			this.RandomizeLobbyBackground();
			this.ResettingCleanup();
			this.IncrementRoundNumber();
			if (!this.LobbyEnabled)
			{
				this.StartRound(false);
				return;
			}
			if (this._playerManager.PlayerCount == 0)
			{
				this._roundStartCountdownHasNotStartedYetDueToNoPlayers = true;
			}
			else
			{
				this._roundStartTime = this._gameTiming.CurTime + this.LobbyDuration;
			}
			this.SendStatusToAll();
			this.UpdateInfoText();
			this.ReqWindowAttentionAll();
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x0007E320 File Offset: 0x0007C520
		private void ResettingCleanup()
		{
			foreach (IPlayerSession player in this._playerManager.ServerSessions)
			{
				this.PlayerJoinLobby(player);
			}
			foreach (IPlayerData data in this._playerManager.GetAllPlayerData())
			{
				PlayerData playerData = data.ContentData();
				if (playerData != null)
				{
					playerData.WipeMind();
				}
			}
			foreach (EntityUid entity in this.EntityManager.GetEntities().ToArray<EntityUid>())
			{
				try
				{
					this.EntityManager.DeleteEntity(entity);
				}
				catch (Exception e)
				{
					ISawmill sawmill = this._sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Caught exception while trying to delete entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(entity));
					defaultInterpolatedStringHandler.AppendLiteral(", this might corrupt the game state...");
					sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					this._runtimeLog.LogException(e, "GameTicker");
				}
			}
			this._mapManager.Restart();
			this._roleBanManager.Restart();
			this._gameMapManager.ClearSelectedMap();
			this.ClearGameRules();
			this._addedGameRules.Clear();
			this._allPreviousGameRules.Clear();
			RoundRestartCleanupEvent ev = new RoundRestartCleanupEvent();
			base.RaiseLocalEvent<RoundRestartCleanupEvent>(ev);
			base.RaiseNetworkEvent(ev, Filter.Broadcast(), true);
			this.DisallowLateJoin = false;
			this._playerGameStatuses.Clear();
			foreach (IPlayerSession session in this._playerManager.ServerSessions)
			{
				this._playerGameStatuses[session.UserId] = (this.LobbyEnabled ? PlayerGameStatus.NotReadyToPlay : PlayerGameStatus.ReadyToPlay);
			}
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0007E524 File Offset: 0x0007C724
		public bool DelayStart(TimeSpan time)
		{
			if (this._runLevel != GameRunLevel.PreRoundLobby)
			{
				return false;
			}
			this._roundStartTime += time;
			base.RaiseNetworkEvent(new TickerLobbyCountdownEvent(this._roundStartTime, this.Paused));
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("game-ticker-delay-start", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seconds", time.TotalSeconds)
			}), null);
			return true;
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x0007E5A8 File Offset: 0x0007C7A8
		private void UpdateRoundFlow(float frameTime)
		{
			if (this.RunLevel == GameRunLevel.InRound)
			{
				GameTicker.RoundLengthMetric.Inc((double)frameTime);
			}
			if (this.RunLevel != GameRunLevel.PreRoundLobby || this.Paused || this._roundStartTime > this._gameTiming.CurTime || this._roundStartCountdownHasNotStartedYetDueToNoPlayers)
			{
				return;
			}
			this.StartRound(false);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0007E604 File Offset: 0x0007C804
		public TimeSpan RoundDuration()
		{
			return this._gameTiming.CurTime.Subtract(this._roundStartTimeSpan);
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0007E62C File Offset: 0x0007C82C
		private void AnnounceRound()
		{
			if (this.Preset == null)
			{
				return;
			}
			foreach (RoundAnnouncementPrototype proto in this._prototypeManager.EnumeratePrototypes<RoundAnnouncementPrototype>())
			{
				if (proto.GamePresets.Contains(this.Preset.ID))
				{
					if (proto.Message != null)
					{
						this._chatSystem.DispatchGlobalAnnouncement(Loc.GetString(proto.Message), "Central Command", true, null, null);
					}
					if (proto.Sound != null)
					{
						SoundSystem.Play(proto.Sound.GetSound(null, null), Filter.Broadcast(), null);
						break;
					}
					break;
				}
			}
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0007E6F4 File Offset: 0x0007C8F4
		private void SpawnPlayers(List<IPlayerSession> readyPlayers, Dictionary<NetUserId, HumanoidCharacterProfile> profiles, bool force)
		{
			base.RaiseLocalEvent<RulePlayerSpawningEvent>(new RulePlayerSpawningEvent(readyPlayers, profiles, force));
			HashSet<NetUserId> playerNetIds = (from o in readyPlayers
			select o.UserId).ToHashSet<NetUserId>();
			if (readyPlayers.Count != profiles.Count)
			{
				RemQueue<NetUserId> toRemove = default(RemQueue<NetUserId>);
				foreach (KeyValuePair<NetUserId, HumanoidCharacterProfile> keyValuePair in profiles)
				{
					NetUserId netUserId;
					HumanoidCharacterProfile humanoidCharacterProfile;
					keyValuePair.Deconstruct(out netUserId, out humanoidCharacterProfile);
					NetUserId player = netUserId;
					if (!playerNetIds.Contains(player))
					{
						toRemove.Add(player);
					}
				}
				foreach (NetUserId player2 in toRemove)
				{
					profiles.Remove(player2);
				}
			}
			Dictionary<NetUserId, ValueTuple<string, EntityUid>> assignedJobs = this._stationJobs.AssignJobs(profiles, this._stationSystem.Stations.ToList<EntityUid>(), true);
			this._stationJobs.AssignOverflowJobs(ref assignedJobs, playerNetIds, profiles, this._stationSystem.Stations.ToList<EntityUid>());
			Dictionary<EntityUid, int> stationJobCounts = this._stationSystem.Stations.ToDictionary((EntityUid e) => e, (EntityUid _) => 0);
			foreach (KeyValuePair<NetUserId, ValueTuple<string, EntityUid>> keyValuePair2 in assignedJobs)
			{
				NetUserId netUserId;
				ValueTuple<string, EntityUid> valueTuple;
				keyValuePair2.Deconstruct(out netUserId, out valueTuple);
				ValueTuple<string, EntityUid> valueTuple2 = valueTuple;
				NetUserId netUser = netUserId;
				string job = valueTuple2.Item1;
				EntityUid station = valueTuple2.Item2;
				if (job == null)
				{
					IPlayerSession playerSession = this._playerManager.GetSessionByUserId(netUser);
					this._chatManager.DispatchServerMessage(playerSession, Loc.GetString("job-not-available-wait-in-lobby"), false);
				}
				else
				{
					Dictionary<EntityUid, int> dictionary = stationJobCounts;
					EntityUid key = station;
					dictionary[key]++;
				}
			}
			this._stationJobs.CalcExtendedAccess(stationJobCounts);
			foreach (KeyValuePair<NetUserId, ValueTuple<string, EntityUid>> keyValuePair2 in assignedJobs)
			{
				NetUserId netUserId;
				ValueTuple<string, EntityUid> valueTuple;
				keyValuePair2.Deconstruct(out netUserId, out valueTuple);
				ValueTuple<string, EntityUid> valueTuple3 = valueTuple;
				NetUserId player3 = netUserId;
				string job2 = valueTuple3.Item1;
				EntityUid station2 = valueTuple3.Item2;
				if (job2 != null)
				{
					this.SpawnPlayer(this._playerManager.GetSessionByUserId(player3), profiles[player3], station2, job2, false);
				}
			}
			this.RefreshLateJoinAllowed();
			base.RaiseLocalEvent<RulePlayerJobsAssignedEvent>(new RulePlayerJobsAssignedEvent((from x in assignedJobs.Keys
			select this._playerManager.GetSessionByUserId(x)).ToArray<IPlayerSession>(), profiles, force));
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x0007E9D8 File Offset: 0x0007CBD8
		private void SpawnPlayer(IPlayerSession player, EntityUid station, [Nullable(2)] string jobId = null, bool lateJoin = true)
		{
			HumanoidCharacterProfile character = this.GetPlayerProfile(player);
			HashSet<string> jobBans = this._roleBanManager.GetJobBans(player.UserId);
			if (jobBans == null || (jobId != null && jobBans.Contains(jobId)))
			{
				return;
			}
			if (jobId != null && !this._playTimeTrackings.IsAllowed(player, jobId))
			{
				return;
			}
			this.SpawnPlayer(player, character, station, jobId, lateJoin);
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0007EA30 File Offset: 0x0007CC30
		private void SpawnPlayer(IPlayerSession player, HumanoidCharacterProfile character, EntityUid station, [Nullable(2)] string jobId = null, bool lateJoin = true)
		{
			if (this.DummyTicker)
			{
				return;
			}
			if (station == EntityUid.Invalid)
			{
				List<EntityUid> stations = this._stationSystem.Stations.ToList<EntityUid>();
				this._robustRandom.Shuffle<EntityUid>(stations);
				if (stations.Count == 0)
				{
					station = EntityUid.Invalid;
				}
				else
				{
					station = stations[0];
				}
			}
			if (lateJoin && this.DisallowLateJoin)
			{
				this.MakeObserve(player);
				return;
			}
			PlayerBeforeSpawnEvent bev = new PlayerBeforeSpawnEvent(player, character, jobId, lateJoin, station);
			base.RaiseLocalEvent<PlayerBeforeSpawnEvent>(bev);
			if (bev.Handled)
			{
				this.PlayerJoinGame(player);
				return;
			}
			HashSet<string> restrictedRoles = new HashSet<string>();
			HashSet<string> getDisallowed = this._playTimeTrackings.GetDisallowedJobs(player);
			restrictedRoles.UnionWith(getDisallowed);
			HashSet<string> jobBans = this._roleBanManager.GetJobBans(player.UserId);
			if (jobBans != null)
			{
				restrictedRoles.UnionWith(jobBans);
			}
			if (jobId == null)
			{
				jobId = this._stationJobs.PickBestAvailableJobWithPriority(station, character.JobPriorities, true, restrictedRoles);
			}
			if (jobId == null)
			{
				if (!this.LobbyEnabled)
				{
					this.MakeObserve(player);
				}
				this._chatManager.DispatchServerMessage(player, Loc.GetString("game-ticker-player-no-jobs-available-when-joining"), false);
				return;
			}
			this.PlayerJoinGame(player);
			PlayerData data = player.ContentData();
			data.WipeMind();
			Mind newMind = new Mind(data.UserId)
			{
				CharacterName = character.Name
			};
			newMind.ChangeOwningPlayer(new NetUserId?(data.UserId));
			JobPrototype jobPrototype = this._prototypeManager.Index<JobPrototype>(jobId);
			Content.Server.Roles.Job job = new Content.Server.Roles.Job(newMind, jobPrototype);
			newMind.AddRole(job);
			if (this._cfg.GetCVar<bool>(CCVars.FanaticXenophobiaEnabled))
			{
				character = this.ReplaceBlacklistedSpecies(player, character, jobPrototype);
				newMind.CharacterName = character.Name;
			}
			this._playTimeTrackings.PlayerRolesChanged(player);
			EntityUid mob = this._stationSpawning.SpawnPlayerCharacterOnStation(new EntityUid?(station), job, character, null).Value;
			newMind.TransferTo(new EntityUid?(mob), false, false);
			if (lateJoin)
			{
				this._chatSystem.DispatchStationAnnouncement(station, Loc.GetString("latejoin-arrival-announcement", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("character", base.MetaData(mob).EntityName),
					new ValueTuple<string, object>("gender", character.Gender),
					new ValueTuple<string, object>("job", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(job.Name))
				}), Loc.GetString("latejoin-arrival-sender"), false, null, null);
			}
			if (player.UserId == new Guid("{e887eb93-f503-4b65-95b6-2f282c014192}"))
			{
				this.EntityManager.AddComponent<OwOAccentComponent>(mob);
			}
			this._stationJobs.TryAssignJob(station, jobPrototype, null);
			if (lateJoin)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.LateJoin;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(48, 5);
				logStringHandler.AppendLiteral("Player ");
				logStringHandler.AppendFormatted(player.Name);
				logStringHandler.AppendLiteral(" late joined as ");
				logStringHandler.AppendFormatted(character.Name, 0, "characterName");
				logStringHandler.AppendLiteral(" on station ");
				logStringHandler.AppendFormatted(base.Name(station, null), 0, "stationName");
				logStringHandler.AppendLiteral(" with ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(mob), "entity", "ToPrettyString(mob)");
				logStringHandler.AppendLiteral(" as a ");
				logStringHandler.AppendFormatted(job.Name, 0, "jobName");
				logStringHandler.AppendLiteral(".");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.RoundStartJoin;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(43, 5);
				logStringHandler.AppendLiteral("Player ");
				logStringHandler.AppendFormatted(player.Name);
				logStringHandler.AppendLiteral(" joined as ");
				logStringHandler.AppendFormatted(character.Name, 0, "characterName");
				logStringHandler.AppendLiteral(" on station ");
				logStringHandler.AppendFormatted(base.Name(station, null), 0, "stationName");
				logStringHandler.AppendLiteral(" with ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(mob), "entity", "ToPrettyString(mob)");
				logStringHandler.AppendLiteral(" as a ");
				logStringHandler.AppendFormatted(job.Name, 0, "jobName");
				logStringHandler.AppendLiteral(".");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			if (base.Comp<StationJobsComponent>(station).ExtendedAccess && (jobPrototype.ExtendedAccess.Count > 0 || jobPrototype.ExtendedAccessGroups.Count > 0))
			{
				this._chatManager.DispatchServerMessage(player, Loc.GetString("job-greet-crew-shortages"), false);
			}
			MetaDataComponent metaData;
			if (base.TryComp<MetaDataComponent>(station, ref metaData))
			{
				this._chatManager.DispatchServerMessage(player, Loc.GetString("job-greet-station-name", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("stationName", metaData.EntityName)
				}), false);
			}
			this.PlayersJoinedRoundNormally++;
			PlayerSpawnCompleteEvent aev = new PlayerSpawnCompleteEvent(mob, player, jobId, lateJoin, this.PlayersJoinedRoundNormally, station, character);
			base.RaiseLocalEvent<PlayerSpawnCompleteEvent>(mob, aev, true);
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x0007EF2C File Offset: 0x0007D12C
		private HumanoidCharacterProfile ReplaceBlacklistedSpecies(IPlayerSession player, HumanoidCharacterProfile character, JobPrototype jobPrototype)
		{
			IReadOnlyCollection<string> whitelistedSpecies = jobPrototype.WhitelistedSpecies;
			if (whitelistedSpecies.Count > 0 && !whitelistedSpecies.Contains(character.Species))
			{
				List<HumanoidCharacterProfile> existedAllowedProfile = this._prefsManager.GetPreferences(player.UserId).Characters.Values.Cast<HumanoidCharacterProfile>().ToList<HumanoidCharacterProfile>().FindAll((HumanoidCharacterProfile x) => whitelistedSpecies.Contains(x.Species));
				if (existedAllowedProfile.Count == 0)
				{
					character = HumanoidCharacterProfile.RandomWithSpecies(RandomExtensions.Pick<string>(this._robustRandom, whitelistedSpecies));
					this._chatManager.DispatchServerMessage(player, "Данному виду запрещено играть на этой профессии. Вам была выдана случайная внешность.", false);
				}
				else
				{
					character = RandomExtensions.Pick<HumanoidCharacterProfile>(this._robustRandom, existedAllowedProfile);
					this._chatManager.DispatchServerMessage(player, "Данному виду запрещено играть на этой профессии. Вам была выдана случайная внешность с подходящим видом из вашего профиля.", false);
				}
				StringBuilder availableSpeciesLoc = new StringBuilder();
				foreach (string specie in whitelistedSpecies)
				{
					availableSpeciesLoc.AppendLine("- " + Loc.GetString("species-name-" + specie.ToLower()));
				}
				IChatManager chatManager = this._chatManager;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Доступные виды: \n ");
				defaultInterpolatedStringHandler.AppendFormatted<StringBuilder>(availableSpeciesLoc);
				chatManager.DispatchServerMessage(player, defaultInterpolatedStringHandler.ToStringAndClear(), false);
			}
			return character;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x0007F098 File Offset: 0x0007D298
		public void Respawn(IPlayerSession player)
		{
			PlayerData playerData = player.ContentData();
			if (playerData != null)
			{
				playerData.WipeMind();
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Respawn;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(22, 1);
			logStringHandler.AppendLiteral("Player ");
			logStringHandler.AppendFormatted<IPlayerSession>(player, "player");
			logStringHandler.AppendLiteral(" was respawned.");
			adminLogger.Add(type, impact, ref logStringHandler);
			if (this.LobbyEnabled)
			{
				this.PlayerJoinLobby(player);
				return;
			}
			this.SpawnPlayer(player, EntityUid.Invalid, null, true);
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0007F113 File Offset: 0x0007D313
		public void MakeJoinGame(IPlayerSession player, EntityUid station, [Nullable(2)] string jobId = null)
		{
			if (!this._playerGameStatuses.ContainsKey(player.UserId))
			{
				return;
			}
			if (!this._userDb.IsLoadComplete(player))
			{
				return;
			}
			this.SpawnPlayer(player, station, jobId, true);
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x0007F144 File Offset: 0x0007D344
		public void MakeObserve(IPlayerSession player)
		{
			GameTicker.<MakeObserve>d__198 <MakeObserve>d__;
			<MakeObserve>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MakeObserve>d__.<>4__this = this;
			<MakeObserve>d__.player = player;
			<MakeObserve>d__.<>1__state = -1;
			<MakeObserve>d__.<>t__builder.Start<GameTicker.<MakeObserve>d__198>(ref <MakeObserve>d__);
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x0007F184 File Offset: 0x0007D384
		private EntityUid SpawnObserverMob()
		{
			EntityCoordinates coordinates = this.GetObserverSpawnPoint();
			return this.EntityManager.SpawnEntity("MobObserver", coordinates);
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0007F1AC File Offset: 0x0007D3AC
		public EntityCoordinates GetObserverSpawnPoint()
		{
			this._possiblePositions.Clear();
			foreach (ValueTuple<SpawnPointComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<SpawnPointComponent, TransformComponent>(true))
			{
				SpawnPointComponent point = valueTuple.Item1;
				TransformComponent transform = valueTuple.Item2;
				if (point.SpawnType == SpawnPointType.Observer)
				{
					this._possiblePositions.Add(transform.Coordinates);
				}
			}
			EntityQuery<MetaDataComponent> metaQuery = base.GetEntityQuery<MetaDataComponent>();
			if (this._possiblePositions.Count == 0)
			{
				foreach (MapGridComponent grid in this._mapManager.GetAllGrids())
				{
					MetaDataComponent meta;
					if (metaQuery.TryGetComponent(grid.Owner, ref meta) && !meta.EntityPaused)
					{
						this._possiblePositions.Add(new EntityCoordinates(grid.Owner, Vector2.Zero));
					}
				}
			}
			if (this._possiblePositions.Count != 0)
			{
				EntityCoordinates spawn = RandomExtensions.Pick<EntityCoordinates>(this._robustRandom, this._possiblePositions);
				MapCoordinates toMap = spawn.ToMap(this.EntityManager);
				MapGridComponent foundGrid;
				if (this._mapManager.TryFindGridAt(toMap, ref foundGrid))
				{
					TransformComponent gridXform = base.Transform(foundGrid.Owner);
					return new EntityCoordinates(foundGrid.Owner, gridXform.InvWorldMatrix.Transform(toMap.Position));
				}
				return spawn;
			}
			else
			{
				if (this._mapManager.MapExists(this.DefaultMap))
				{
					return new EntityCoordinates(this._mapManager.GetMapEntityId(this.DefaultMap), Vector2.Zero);
				}
				foreach (MapId map in this._mapManager.GetAllMapIds())
				{
					EntityUid mapUid = this._mapManager.GetMapEntityId(map);
					MetaDataComponent meta2;
					if (metaQuery.TryGetComponent(mapUid, ref meta2) && !meta2.EntityPaused)
					{
						return new EntityCoordinates(mapUid, Vector2.Zero);
					}
				}
				this._sawmill.Warning("Found no observer spawn points!");
				return EntityCoordinates.Invalid;
			}
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0007F3E8 File Offset: 0x0007D5E8
		private void InitializeStatusShell()
		{
			IoCManager.Resolve<IStatusHost>().OnStatusRequest += this.GetStatusResponse;
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x0007F400 File Offset: 0x0007D600
		private void GetStatusResponse(JsonNode jObject)
		{
			object statusShellLock = this._statusShellLock;
			lock (statusShellLock)
			{
				jObject["name"] = this._baseServer.ServerName;
				jObject["players"] = this._queueManager.ActualPlayersCount;
				jObject["soft_max_players"] = this._cfg.GetCVar<int>(CCVars.SoftMaxPlayers);
				jObject["run_level"] = (int)this._runLevel;
				if (this._runLevel >= GameRunLevel.InRound)
				{
					jObject["round_start_time"] = this._roundStartDateTime.ToString("o");
				}
			}
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x0007F658 File Offset: 0x0007D858
		[CompilerGenerated]
		private void <StartPreset>g__FailedPresetRestart|79_0(ref GameTicker.<>c__DisplayClass79_0 A_1)
		{
			this.SendServerMessage(Loc.GetString("game-ticker-start-round-cannot-start-game-mode-restart", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("failedGameMode", A_1.presetTitle)
			}));
			this.RestartRound();
			this.DelayStart(TimeSpan.FromSeconds(30.0));
		}

		// Token: 0x04000ED1 RID: 3793
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x04000ED2 RID: 3794
		[Dependency]
		private readonly MobStateSystem _mobState;

		// Token: 0x04000ED3 RID: 3795
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x04000ED4 RID: 3796
		[ViewVariables]
		private bool _initialized;

		// Token: 0x04000ED5 RID: 3797
		[ViewVariables]
		private bool _postInitialized;

		// Token: 0x04000ED7 RID: 3799
		private ISawmill _sawmill;

		// Token: 0x04000ED8 RID: 3800
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000ED9 RID: 3801
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000EDA RID: 3802
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000EDB RID: 3803
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000EDC RID: 3804
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000EDD RID: 3805
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000EDE RID: 3806
		[Dependency]
		private readonly IServerPreferencesManager _prefsManager;

		// Token: 0x04000EDF RID: 3807
		[Dependency]
		private readonly IBaseServer _baseServer;

		// Token: 0x04000EE0 RID: 3808
		[Dependency]
		private readonly IGameMapManager _gameMapManager;

		// Token: 0x04000EE1 RID: 3809
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04000EE2 RID: 3810
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000EE3 RID: 3811
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04000EE4 RID: 3812
		[Dependency]
		private readonly IConsoleHost _consoleHost;

		// Token: 0x04000EE5 RID: 3813
		[Dependency]
		private readonly IRuntimeLog _runtimeLog;

		// Token: 0x04000EE6 RID: 3814
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000EE7 RID: 3815
		[Dependency]
		private readonly StationSpawningSystem _stationSpawning;

		// Token: 0x04000EE8 RID: 3816
		[Dependency]
		private readonly StationJobsSystem _stationJobs;

		// Token: 0x04000EE9 RID: 3817
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x04000EEA RID: 3818
		[Dependency]
		private readonly GhostSystem _ghosts;

		// Token: 0x04000EEB RID: 3819
		[Dependency]
		private readonly RoleBanManager _roleBanManager;

		// Token: 0x04000EEC RID: 3820
		[Dependency]
		private readonly ChatSystem _chatSystem;

		// Token: 0x04000EED RID: 3821
		[Dependency]
		private readonly ServerUpdateManager _serverUpdates;

		// Token: 0x04000EEE RID: 3822
		[Dependency]
		private readonly PlayTimeTrackingSystem _playTimeTrackings;

		// Token: 0x04000EEF RID: 3823
		[Dependency]
		private readonly UserDbDataManager _userDb;

		// Token: 0x04000EF8 RID: 3832
		public const float PresetFailedCooldownIncrease = 30f;

		// Token: 0x04000EF9 RID: 3833
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000EFA RID: 3834
		[Dependency]
		private readonly GhostSystem _ghostSystem;

		// Token: 0x04000EFC RID: 3836
		[ViewVariables]
		private readonly HashSet<GameRulePrototype> _addedGameRules = new HashSet<GameRulePrototype>();

		// Token: 0x04000EFD RID: 3837
		[ViewVariables]
		private readonly HashSet<GameRulePrototype> _startedGameRules = new HashSet<GameRulePrototype>();

		// Token: 0x04000EFE RID: 3838
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		[ViewVariables]
		private readonly List<ValueTuple<TimeSpan, GameRulePrototype>> _allPreviousGameRules = new List<ValueTuple<TimeSpan, GameRulePrototype>>();

		// Token: 0x04000EFF RID: 3839
		[ViewVariables]
		private readonly Dictionary<NetUserId, PlayerGameStatus> _playerGameStatuses = new Dictionary<NetUserId, PlayerGameStatus>();

		// Token: 0x04000F00 RID: 3840
		[ViewVariables]
		private TimeSpan _roundStartTime;

		// Token: 0x04000F01 RID: 3841
		[ViewVariables]
		private TimeSpan _pauseTime;

		// Token: 0x04000F03 RID: 3843
		[ViewVariables]
		private bool _roundStartCountdownHasNotStartedYetDueToNoPlayers;

		// Token: 0x04000F05 RID: 3845
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ViewVariables]
		private List<ResourcePath> _lobbyBackgrounds;

		// Token: 0x04000F06 RID: 3846
		private static readonly string[] WhitelistedBackgroundExtensions = new string[]
		{
			"png",
			"jpg",
			"jpeg"
		};

		// Token: 0x04000F07 RID: 3847
		private const string LobbyMusicCollection = "LobbyMusic";

		// Token: 0x04000F08 RID: 3848
		[ViewVariables]
		private bool _lobbyMusicInitialized;

		// Token: 0x04000F09 RID: 3849
		[ViewVariables]
		private SoundCollectionPrototype _lobbyMusicCollection;

		// Token: 0x04000F0B RID: 3851
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000F0C RID: 3852
		[Dependency]
		private readonly IServerDbManager _dbManager;

		// Token: 0x04000F0D RID: 3853
		[Dependency]
		private readonly ITaskManager _taskManager;

		// Token: 0x04000F0E RID: 3854
		[Dependency]
		private readonly StalinManager _stalinManager;

		// Token: 0x04000F0F RID: 3855
		private static readonly Counter RoundNumberMetric = Metrics.CreateCounter("ss14_round_number", "Round number.", null);

		// Token: 0x04000F10 RID: 3856
		private static readonly Gauge RoundLengthMetric = Metrics.CreateGauge("ss14_round_length", "Round length in seconds.", null);

		// Token: 0x04000F11 RID: 3857
		[ViewVariables]
		private int _roundStartFailCount;

		// Token: 0x04000F12 RID: 3858
		[ViewVariables]
		private TimeSpan _roundStartTimeSpan;

		// Token: 0x04000F13 RID: 3859
		[ViewVariables]
		private bool _startingRound;

		// Token: 0x04000F14 RID: 3860
		[ViewVariables]
		private GameRunLevel _runLevel;

		// Token: 0x04000F16 RID: 3862
		private const string ObserverPrototypeName = "MobObserver";

		// Token: 0x04000F17 RID: 3863
		public int PlayersJoinedRoundNormally;

		// Token: 0x04000F18 RID: 3864
		private readonly List<EntityCoordinates> _possiblePositions = new List<EntityCoordinates>();

		// Token: 0x04000F19 RID: 3865
		private readonly object _statusShellLock = new object();

		// Token: 0x04000F1A RID: 3866
		[ViewVariables]
		private DateTime _roundStartDateTime;

		// Token: 0x04000F1B RID: 3867
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000F1C RID: 3868
		[Dependency]
		private readonly JoinQueueManager _queueManager;
	}
}
