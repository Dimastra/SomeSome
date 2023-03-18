using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.Chat.Managers;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Server.Station.Components;
using Content.Server.Suspicion;
using Content.Server.Suspicion.Roles;
using Content.Server.Traitor.Uplink;
using Content.Shared.CCVar;
using Content.Shared.Doors.Systems;
using Content.Shared.EntityList;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Roles;
using Content.Shared.Suspicion;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004C1 RID: 1217
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuspicionRuleSystem : GameRuleSystem
	{
		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06001909 RID: 6409 RVA: 0x00083404 File Offset: 0x00081604
		public override string Prototype
		{
			get
			{
				return "Suspicion";
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x0008340B File Offset: 0x0008160B
		public IReadOnlyCollection<SuspicionRoleComponent> Traitors
		{
			get
			{
				return this._traitors;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600190B RID: 6411 RVA: 0x00083413 File Offset: 0x00081613
		// (set) Token: 0x0600190C RID: 6412 RVA: 0x0008341B File Offset: 0x0008161B
		public TimeSpan? EndTime
		{
			get
			{
				return this._endTime;
			}
			set
			{
				this._endTime = value;
				this.SendUpdateToAll();
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x0600190D RID: 6413 RVA: 0x0008342A File Offset: 0x0008162A
		// (set) Token: 0x0600190E RID: 6414 RVA: 0x00083432 File Offset: 0x00081632
		public TimeSpan RoundMaxTime { get; set; } = TimeSpan.FromSeconds((double)CCVars.SuspicionMaxTimeSeconds.DefaultValue);

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x0600190F RID: 6415 RVA: 0x0008343B File Offset: 0x0008163B
		// (set) Token: 0x06001910 RID: 6416 RVA: 0x00083443 File Offset: 0x00081643
		public TimeSpan RoundEndDelay { get; set; } = TimeSpan.FromSeconds(10.0);

		// Token: 0x06001911 RID: 6417 RVA: 0x0008344C File Offset: 0x0008164C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(new EntityEventHandler<RulePlayerJobsAssignedEvent>(this.OnPlayersAssigned), null, null);
			base.SubscribeLocalEvent<RoundStartAttemptEvent>(new EntityEventHandler<RoundStartAttemptEvent>(this.OnRoundStartAttempt), null, null);
			base.SubscribeLocalEvent<RefreshLateJoinAllowedEvent>(new EntityEventHandler<RefreshLateJoinAllowedEvent>(this.OnLateJoinRefresh), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, PlayerAttachedEvent>(new ComponentEventHandler<SuspicionRoleComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, PlayerDetachedEvent>(new ComponentEventHandler<SuspicionRoleComponent, PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, RoleAddedEvent>(new ComponentEventHandler<SuspicionRoleComponent, RoleAddedEvent>(this.OnRoleAdded), null, null);
			base.SubscribeLocalEvent<SuspicionRoleComponent, RoleRemovedEvent>(new ComponentEventHandler<SuspicionRoleComponent, RoleRemovedEvent>(this.OnRoleRemoved), null, null);
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x00083500 File Offset: 0x00081700
		private void OnRoundStartAttempt(RoundStartAttemptEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			int minPlayers = this._cfg.GetCVar<int>(CCVars.SuspicionMinPlayers);
			if (!ev.Forced && ev.Players.Length < minPlayers)
			{
				IChatManager chatManager = this._chatManager;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(90, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Not enough players readied up for the game! There were ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(ev.Players.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" players readied up out of ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(minPlayers);
				defaultInterpolatedStringHandler.AppendLiteral(" needed.");
				chatManager.DispatchServerAnnouncement(defaultInterpolatedStringHandler.ToStringAndClear(), null);
				ev.Cancel();
				return;
			}
			if (ev.Players.Length == 0)
			{
				this._chatManager.DispatchServerAnnouncement("No players readied up! Can't start Suspicion.", null);
				ev.Cancel();
			}
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x000835CC File Offset: 0x000817CC
		private void OnPlayersAssigned(RulePlayerJobsAssignedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			int minTraitors = this._cfg.GetCVar<int>(CCVars.SuspicionMinTraitors);
			int playersPerTraitor = this._cfg.GetCVar<int>(CCVars.SuspicionPlayersPerTraitor);
			int traitorStartingBalance = this._cfg.GetCVar<int>(CCVars.SuspicionStartingBalance);
			List<IPlayerSession> list = new List<IPlayerSession>(ev.Players);
			List<IPlayerSession> prefList = new List<IPlayerSession>();
			foreach (IPlayerSession player in list)
			{
				if (ev.Profiles.ContainsKey(player.UserId))
				{
					EntityUid? attachedEntity = player.AttachedEntity;
					if (attachedEntity != null)
					{
						EntityUid attached = attachedEntity.GetValueOrDefault();
						prefList.Add(player);
						ComponentExt.EnsureComponent<SuspicionRoleComponent>(attached);
					}
				}
			}
			int numTraitors = MathHelper.Clamp(ev.Players.Length / playersPerTraitor, minTraitors, ev.Players.Length - 1);
			List<SuspicionTraitorRole> traitors = new List<SuspicionTraitorRole>();
			for (int i = 0; i < numTraitors; i++)
			{
				IPlayerSession traitor;
				if (prefList.Count == 0)
				{
					if (list.Count == 0)
					{
						Logger.InfoS("preset", "Insufficient ready players to fill up with traitors, stopping the selection.");
						break;
					}
					traitor = RandomExtensions.PickAndTake<IPlayerSession>(this._random, list);
					Logger.InfoS("preset", "Insufficient preferred traitors, picking at random.");
				}
				else
				{
					traitor = RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList);
					list.Remove(traitor);
					Logger.InfoS("preset", "Selected a preferred traitor.");
				}
				PlayerData playerData = traitor.Data.ContentData();
				Mind mind = (playerData != null) ? playerData.Mind : null;
				AntagPrototype antagPrototype = this._prototypeManager.Index<AntagPrototype>("SuspicionTraitor");
				SuspicionTraitorRole traitorRole = new SuspicionTraitorRole(mind, antagPrototype);
				mind.AddRole(traitorRole);
				traitors.Add(traitorRole);
				this._uplink.AddUplink(mind.OwnedEntity.Value, new FixedPoint2?(traitorStartingBalance), "StorePresetUplink", null);
			}
			foreach (IPlayerSession playerSession in list)
			{
				PlayerData playerData2 = playerSession.Data.ContentData();
				object obj = (playerData2 != null) ? playerData2.Mind : null;
				AntagPrototype antagPrototype2 = this._prototypeManager.Index<AntagPrototype>("SuspicionInnocent");
				object obj2 = obj;
				obj2.AddRole(new SuspicionInnocentRole(obj2, antagPrototype2));
			}
			foreach (SuspicionTraitorRole suspicionTraitorRole in traitors)
			{
				suspicionTraitorRole.GreetSuspicion(traitors, this._chatManager);
			}
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x0008387C File Offset: 0x00081A7C
		public override void Started()
		{
			this._playerManager.PlayerStatusChanged += this.PlayerManagerOnPlayerStatusChanged;
			this.RoundMaxTime = TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.SuspicionMaxTimeSeconds));
			this.EndTime = new TimeSpan?(this._timing.CurTime + this.RoundMaxTime);
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-suspicion-added-announcement"), null);
			Filter filter = Filter.Empty().AddWhere(delegate(ICommonSession session)
			{
				PlayerData playerData = ((IPlayerSession)session).ContentData();
				bool? flag;
				if (playerData == null)
				{
					flag = null;
				}
				else
				{
					Mind mind = playerData.Mind;
					flag = ((mind != null) ? new bool?(mind.HasRole<SuspicionTraitorRole>()) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}, null);
			SoundSystem.Play(this._addedSound.GetSound(null, null), filter, new AudioParams?(AudioParams.Default));
			this._doorSystem.AccessType = SharedDoorSystem.AccessTypes.AllowAllNoExternal;
			EntityLootTablePrototype susLoot = this._prototypeManager.Index<EntityLootTablePrototype>("SuspicionRule");
			foreach (ValueTuple<StationMemberComponent, MapGridComponent> valueTuple in this.EntityManager.EntityQuery<StationMemberComponent, MapGridComponent>(true))
			{
				TileRef[] tiles = valueTuple.Item2.GetAllTiles(true).ToArray<TileRef>();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("TILES: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(tiles.Length);
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				List<string> spawn = susLoot.GetSpawns(null);
				int count = spawn.Count;
				if (tiles.Length < 1000)
				{
					count = Math.Min(count, tiles.Length / 10);
					this._random.Shuffle<string>(spawn);
				}
				for (int i = 0; i < count; i++)
				{
					string item = spawn[i];
					for (int j = 0; j < 100; j++)
					{
						TileRef tile = RandomExtensions.Pick<TileRef>(this._random, tiles);
						if (!tile.IsBlockedTurf(false, this._lookupSystem, null) && !tile.IsSpace(this._tileDefMan))
						{
							EntityUid uid = base.Spawn(item, tile.GridPosition(this._mapManager));
							base.EnsureComp<SuspicionItemComponent>(uid);
							break;
						}
					}
				}
			}
			this._checkTimerCancel = new CancellationTokenSource();
			Timer.SpawnRepeating(SuspicionRuleSystem.DeadCheckDelay, new Action(this.CheckWinConditions), this._checkTimerCancel.Token);
		}

		// Token: 0x06001915 RID: 6421 RVA: 0x00083AD4 File Offset: 0x00081CD4
		public override void Ended()
		{
			this._doorSystem.AccessType = SharedDoorSystem.AccessTypes.Id;
			this.EndTime = null;
			this._traitors.Clear();
			this._playerManager.PlayerStatusChanged -= this.PlayerManagerOnPlayerStatusChanged;
			foreach (SuspicionItemComponent item in this.EntityManager.EntityQuery<SuspicionItemComponent>(true))
			{
				base.Del(item.Owner);
			}
			this._checkTimerCancel.Cancel();
		}

		// Token: 0x06001916 RID: 6422 RVA: 0x00083B74 File Offset: 0x00081D74
		private void CheckWinConditions()
		{
			if (!base.RuleAdded || !this._cfg.GetCVar<bool>(CCVars.GameLobbyEnableWin))
			{
				return;
			}
			int traitorsAlive = 0;
			int innocentsAlive = 0;
			foreach (IPlayerSession playerSession in this._playerManager.ServerSessions)
			{
				EntityUid? attachedEntity = playerSession.AttachedEntity;
				if (attachedEntity != null)
				{
					EntityUid playerEntity = attachedEntity.GetValueOrDefault();
					MobStateComponent mobState;
					if (playerEntity.Valid && base.TryComp<MobStateComponent>(playerEntity, ref mobState) && base.HasComp<SuspicionRoleComponent>(playerEntity) && this._mobStateSystem.IsAlive(playerEntity, mobState))
					{
						PlayerData playerData = playerSession.ContentData();
						Mind mind = (playerData != null) ? playerData.Mind : null;
						if (mind != null && mind.HasRole<SuspicionTraitorRole>())
						{
							traitorsAlive++;
						}
						else
						{
							innocentsAlive++;
						}
					}
				}
			}
			if (innocentsAlive + traitorsAlive == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-suspicion-check-winner-stalemate"), null);
				this.EndRound(SuspicionRuleSystem.Victory.Stalemate);
				return;
			}
			if (traitorsAlive == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-suspicion-check-winner-station-win"), null);
				this.EndRound(SuspicionRuleSystem.Victory.Innocents);
				return;
			}
			if (innocentsAlive == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-suspicion-check-winner-traitor-win"), null);
				this.EndRound(SuspicionRuleSystem.Victory.Traitors);
				return;
			}
			if (this._timing.CurTime > this._endTime)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-suspicion-traitor-time-has-run-out"), null);
				this.EndRound(SuspicionRuleSystem.Victory.Innocents);
			}
		}

		// Token: 0x06001917 RID: 6423 RVA: 0x00083D30 File Offset: 0x00081F30
		private void EndRound(SuspicionRuleSystem.Victory victory)
		{
			string text;
			if (victory != SuspicionRuleSystem.Victory.Innocents)
			{
				if (victory != SuspicionRuleSystem.Victory.Traitors)
				{
					text = Loc.GetString("rule-suspicion-end-round-nobody-victory");
				}
				else
				{
					text = Loc.GetString("rule-suspicion-end-round-traitors-victory");
				}
			}
			else
			{
				text = Loc.GetString("rule-suspicion-end-round-innocents-victory");
			}
			this.GameTicker.EndRound(text);
			this._chatManager.DispatchServerAnnouncement(Loc.GetString("rule-restarting-in-seconds", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("seconds", (int)this.RoundEndDelay.TotalSeconds)
			}), null);
			this._checkTimerCancel.Cancel();
			Timer.Spawn(this.RoundEndDelay, delegate()
			{
				this.GameTicker.RestartRound();
			}, default(CancellationToken));
		}

		// Token: 0x06001918 RID: 6424 RVA: 0x00083DEC File Offset: 0x00081FEC
		private void PlayerManagerOnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 3)
			{
				this.SendUpdateTimerMessage(e.Session);
			}
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x00083E04 File Offset: 0x00082004
		private void SendUpdateToAll()
		{
			foreach (IPlayerSession player in from p in this._playerManager.ServerSessions
			where p.Status == 3
			select p)
			{
				this.SendUpdateTimerMessage(player);
			}
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x00083E7C File Offset: 0x0008207C
		private void SendUpdateTimerMessage(IPlayerSession player)
		{
			SuspicionMessages.SetSuspicionEndTimerMessage msg = new SuspicionMessages.SetSuspicionEndTimerMessage
			{
				EndTime = this.EndTime
			};
			IEntityNetworkManager entityNetManager = this.EntityManager.EntityNetManager;
			if (entityNetManager == null)
			{
				return;
			}
			entityNetManager.SendSystemNetworkMessage(msg, player.ConnectedClient);
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x00083EB8 File Offset: 0x000820B8
		public void AddTraitor(SuspicionRoleComponent role)
		{
			if (!this._traitors.Add(role))
			{
				return;
			}
			foreach (SuspicionRoleComponent suspicionRoleComponent in this._traitors)
			{
				suspicionRoleComponent.AddAlly(role);
			}
			role.SetAllies(this._traitors);
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00083F24 File Offset: 0x00082124
		public void RemoveTraitor(SuspicionRoleComponent role)
		{
			if (!this._traitors.Remove(role))
			{
				return;
			}
			foreach (SuspicionRoleComponent suspicionRoleComponent in this._traitors)
			{
				suspicionRoleComponent.RemoveAlly(role);
			}
			role.ClearAllies();
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x00083F8C File Offset: 0x0008218C
		private void Reset(RoundRestartCleanupEvent ev)
		{
			this.EndTime = null;
			this._traitors.Clear();
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x00083FB3 File Offset: 0x000821B3
		private void OnPlayerDetached(EntityUid uid, SuspicionRoleComponent component, PlayerDetachedEvent args)
		{
			component.SyncRoles();
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x00083FBB File Offset: 0x000821BB
		private void OnPlayerAttached(EntityUid uid, SuspicionRoleComponent component, PlayerAttachedEvent args)
		{
			component.SyncRoles();
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x00083FC4 File Offset: 0x000821C4
		private void OnRoleAdded(EntityUid uid, SuspicionRoleComponent component, RoleAddedEvent args)
		{
			SuspicionRole role = args.Role as SuspicionRole;
			if (role == null)
			{
				return;
			}
			component.Role = role;
		}

		// Token: 0x06001921 RID: 6433 RVA: 0x00083FE8 File Offset: 0x000821E8
		private void OnRoleRemoved(EntityUid uid, SuspicionRoleComponent component, RoleRemovedEvent args)
		{
			if (!(args.Role is SuspicionRole))
			{
				return;
			}
			component.Role = null;
		}

		// Token: 0x06001922 RID: 6434 RVA: 0x00083FFF File Offset: 0x000821FF
		private void OnLateJoinRefresh(RefreshLateJoinAllowedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			ev.Disallow();
		}

		// Token: 0x04000FA0 RID: 4000
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000FA1 RID: 4001
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000FA2 RID: 4002
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000FA3 RID: 4003
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000FA4 RID: 4004
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000FA5 RID: 4005
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000FA6 RID: 4006
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000FA7 RID: 4007
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000FA8 RID: 4008
		[Dependency]
		private readonly ITileDefinitionManager _tileDefMan;

		// Token: 0x04000FA9 RID: 4009
		[Dependency]
		private readonly SharedDoorSystem _doorSystem;

		// Token: 0x04000FAA RID: 4010
		[Dependency]
		private readonly EntityLookupSystem _lookupSystem;

		// Token: 0x04000FAB RID: 4011
		[Dependency]
		private readonly UplinkSystem _uplink;

		// Token: 0x04000FAC RID: 4012
		private static readonly TimeSpan DeadCheckDelay = TimeSpan.FromSeconds(1.0);

		// Token: 0x04000FAD RID: 4013
		private readonly HashSet<SuspicionRoleComponent> _traitors = new HashSet<SuspicionRoleComponent>();

		// Token: 0x04000FAE RID: 4014
		[DataField("addedSound", false, 1, false, false, null)]
		private SoundSpecifier _addedSound = new SoundPathSpecifier("/Audio/Misc/tatoralert.ogg", null);

		// Token: 0x04000FAF RID: 4015
		private CancellationTokenSource _checkTimerCancel = new CancellationTokenSource();

		// Token: 0x04000FB0 RID: 4016
		private TimeSpan? _endTime;

		// Token: 0x04000FB3 RID: 4019
		private const string TraitorID = "SuspicionTraitor";

		// Token: 0x04000FB4 RID: 4020
		private const string InnocentID = "SuspicionInnocent";

		// Token: 0x04000FB5 RID: 4021
		private const string SuspicionLootTable = "SuspicionRule";

		// Token: 0x020009EF RID: 2543
		[NullableContext(0)]
		private enum Victory
		{
			// Token: 0x040022B2 RID: 8882
			Stalemate,
			// Token: 0x040022B3 RID: 8883
			Innocents,
			// Token: 0x040022B4 RID: 8884
			Traitors
		}
	}
}
