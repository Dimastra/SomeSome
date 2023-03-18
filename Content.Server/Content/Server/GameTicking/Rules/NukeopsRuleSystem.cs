using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Commands;
using Content.Server.CharacterAppearance.Components;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Presets;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Ghost.Roles.Events;
using Content.Server.Humanoid.Systems;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.NPC.Systems;
using Content.Server.Nuke;
using Content.Server.Preferences.Managers;
using Content.Server.RoundEnd;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Server.Spawners.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Traitor;
using Content.Server.Traits.Assorted;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Dataset;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Nuke;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Server.Maps;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BC RID: 1212
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeopsRuleSystem : GameRuleSystem
	{
		// Token: 0x17000386 RID: 902
		// (get) Token: 0x060018C4 RID: 6340 RVA: 0x00080235 File Offset: 0x0007E435
		// (set) Token: 0x060018C5 RID: 6341 RVA: 0x0008023D File Offset: 0x0007E43D
		private NukeopsRuleSystem.WinType RuleWinType
		{
			get
			{
				return this._winType;
			}
			set
			{
				this._winType = value;
				if (value == NukeopsRuleSystem.WinType.CrewMajor || value == NukeopsRuleSystem.WinType.OpsMajor)
				{
					this._roundEndSystem.EndRound();
				}
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x060018C6 RID: 6342 RVA: 0x00080258 File Offset: 0x0007E458
		public override string Prototype
		{
			get
			{
				return "Nukeops";
			}
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00080260 File Offset: 0x0007E460
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundStartAttemptEvent>(new EntityEventHandler<RoundStartAttemptEvent>(this.OnStartAttempt), null, null);
			base.SubscribeLocalEvent<RulePlayerSpawningEvent>(new EntityEventHandler<RulePlayerSpawningEvent>(this.OnPlayersSpawning), null, null);
			base.SubscribeLocalEvent<NukeOperativeComponent, MobStateChangedEvent>(new ComponentEventHandler<NukeOperativeComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndText), null, null);
			base.SubscribeLocalEvent<NukeExplodedEvent>(new EntityEventHandler<NukeExplodedEvent>(this.OnNukeExploded), null, null);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRunLevelChanged), null, null);
			base.SubscribeLocalEvent<NukeDisarmSuccessEvent>(new EntityEventHandler<NukeDisarmSuccessEvent>(this.OnNukeDisarm), null, null);
			base.SubscribeLocalEvent<NukeOperativeComponent, GhostRoleSpawnerUsedEvent>(new ComponentEventHandler<NukeOperativeComponent, GhostRoleSpawnerUsedEvent>(this.OnPlayersGhostSpawning), null, null);
			base.SubscribeLocalEvent<NukeOperativeComponent, MindAddedMessage>(new ComponentEventHandler<NukeOperativeComponent, MindAddedMessage>(this.OnMindAdded), null, null);
			base.SubscribeLocalEvent<NukeOperativeComponent, ComponentInit>(new ComponentEventHandler<NukeOperativeComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<NukeOperativeComponent, ComponentRemove>(new ComponentEventHandler<NukeOperativeComponent, ComponentRemove>(this.OnComponentRemove), null, null);
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x00080350 File Offset: 0x0007E550
		private void OnComponentInit(EntityUid uid, NukeOperativeComponent component, ComponentInit args)
		{
			MindComponent mindComponent;
			if (!base.TryComp<MindComponent>(uid, ref mindComponent) || !base.RuleAdded)
			{
				return;
			}
			Mind mind = mindComponent.Mind;
			IPlayerSession session = (mind != null) ? mind.Session : null;
			string name = base.MetaData(uid).EntityName;
			if (session != null)
			{
				this._operativePlayers.Add(name, session);
			}
			base.RemComp<PacifistComponent>(uid);
			base.RemComp<PacifiedComponent>(uid);
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x000803B1 File Offset: 0x0007E5B1
		private void OnComponentRemove(EntityUid uid, NukeOperativeComponent component, ComponentRemove args)
		{
			this.CheckRoundShouldEnd();
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x000803BC File Offset: 0x0007E5BC
		private void OnNukeExploded(NukeExplodedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (ev.OwningStation != null)
			{
				if (ev.OwningStation == this._nukieOutpost)
				{
					this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeExplodedOnNukieOutpost);
					this.RuleWinType = NukeopsRuleSystem.WinType.CrewMajor;
					return;
				}
				StationDataComponent data;
				if (base.TryComp<StationDataComponent>(this._targetStation, ref data))
				{
					using (HashSet<EntityUid>.Enumerator enumerator = data.Grids.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!(enumerator.Current != ev.OwningStation))
							{
								this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeExplodedOnCorrectStation);
								this.RuleWinType = NukeopsRuleSystem.WinType.OpsMajor;
								return;
							}
						}
					}
				}
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeExplodedOnIncorrectLocation);
			}
			else
			{
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeExplodedOnIncorrectLocation);
			}
			this._roundEndSystem.EndRound();
		}

		// Token: 0x060018CB RID: 6347 RVA: 0x000804E8 File Offset: 0x0007E6E8
		private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
		{
			GameRunLevel @new = ev.New;
			if (@new == GameRunLevel.InRound)
			{
				this.OnRoundStart();
				return;
			}
			if (@new != GameRunLevel.PostRound)
			{
				return;
			}
			this.OnRoundEnd();
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x00080514 File Offset: 0x0007E714
		private void OnRoundStart()
		{
			this._targetStation = Extensions.FirstOrNull<EntityUid>(this._stationSystem.Stations);
			if (this._targetStation == null)
			{
				return;
			}
			Filter filter = Filter.Empty();
			foreach (NukeOperativeComponent nukie in base.EntityQuery<NukeOperativeComponent>(false))
			{
				ActorComponent actor;
				if (base.TryComp<ActorComponent>(nukie.Owner, ref actor))
				{
					this._chatManager.DispatchServerMessage(actor.PlayerSession, Loc.GetString("nukeops-welcome", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("station", this._targetStation.Value)
					}), false);
					filter.AddPlayer(actor.PlayerSession);
				}
			}
			this._audioSystem.PlayGlobal(this._nukeopsRuleConfig.GreetSound, filter, false, null);
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x00080608 File Offset: 0x0007E808
		private void OnRoundEnd()
		{
			if (this.RuleWinType == NukeopsRuleSystem.WinType.OpsMajor || this.RuleWinType == NukeopsRuleSystem.WinType.CrewMajor)
			{
				return;
			}
			foreach (ValueTuple<NukeComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<NukeComponent, TransformComponent>(true))
			{
				NukeComponent nuke = valueTuple.Item1;
				TransformComponent nukeTransform = valueTuple.Item2;
				if (nuke.Status == NukeStatus.ARMED)
				{
					if (nukeTransform.MapID == this._shuttleSystem.CentComMap)
					{
						this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeActiveAtCentCom);
						this.RuleWinType = NukeopsRuleSystem.WinType.OpsMajor;
						return;
					}
					StationDataComponent data;
					if (nukeTransform.GridUid != null && this._targetStation != null && base.TryComp<StationDataComponent>(this._targetStation.Value, ref data))
					{
						using (HashSet<EntityUid>.Enumerator enumerator2 = data.Grids.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (!(enumerator2.Current != nukeTransform.GridUid))
								{
									this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeActiveInStation);
									this.RuleWinType = NukeopsRuleSystem.WinType.OpsMajor;
									return;
								}
							}
						}
					}
				}
			}
			bool allAlive = true;
			using (IEnumerator<ValueTuple<NukeOperativeComponent, MobStateComponent>> enumerator3 = base.EntityQuery<NukeOperativeComponent, MobStateComponent>(false).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (enumerator3.Current.Item2.CurrentState != MobState.Alive)
					{
						allAlive = false;
						break;
					}
				}
			}
			if (allAlive)
			{
				this.RuleWinType = NukeopsRuleSystem.WinType.OpsMinor;
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.AllNukiesAlive);
				return;
			}
			this._winConditions.Add(NukeopsRuleSystem.WinCondition.SomeNukiesAlive);
			bool diskAtCentCom = false;
			using (IEnumerator<ValueTuple<NukeDiskComponent, TransformComponent>> enumerator4 = this.EntityManager.EntityQuery<NukeDiskComponent, TransformComponent>(false).GetEnumerator())
			{
				if (enumerator4.MoveNext())
				{
					MapId mapID = enumerator4.Current.Item2.MapID;
					MapId? centComMap = this._shuttleSystem.CentComMap;
					MapId mapId = mapID;
					diskAtCentCom = (centComMap != null && (centComMap == null || centComMap.GetValueOrDefault() == mapId));
				}
			}
			if (diskAtCentCom)
			{
				this.RuleWinType = NukeopsRuleSystem.WinType.CrewMinor;
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeDiskOnCentCom);
				return;
			}
			this.RuleWinType = NukeopsRuleSystem.WinType.OpsMinor;
			this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukeDiskNotOnCentCom);
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000808D4 File Offset: 0x0007EAD4
		private void OnRoundEndText(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			string winText = Loc.GetString("nukeops-" + this._winType.ToString().ToLower());
			ev.AddLine(winText);
			foreach (NukeopsRuleSystem.WinCondition cond in this._winConditions)
			{
				string text = Loc.GetString("nukeops-cond-" + cond.ToString().ToLower());
				ev.AddLine(text);
			}
			ev.AddLine(Loc.GetString("nukeops-list-start"));
			foreach (KeyValuePair<string, IPlayerSession> keyValuePair in this._operativePlayers)
			{
				string text2;
				IPlayerSession playerSession;
				keyValuePair.Deconstruct(out text2, out playerSession);
				string name = text2;
				IPlayerSession session = playerSession;
				string listing = Loc.GetString("nukeops-list-name", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", name),
					new ValueTuple<string, object>("user", session.Name)
				});
				ev.AddLine(listing);
			}
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x00080A24 File Offset: 0x0007EC24
		private void CheckRoundShouldEnd()
		{
			if (!base.RuleAdded || this.RuleWinType == NukeopsRuleSystem.WinType.CrewMajor || this.RuleWinType == NukeopsRuleSystem.WinType.OpsMajor)
			{
				return;
			}
			using (IEnumerator<NukeComponent> enumerator = base.EntityQuery<NukeComponent>(false).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Status == NukeStatus.ARMED)
					{
						return;
					}
				}
			}
			MapId? shuttleMapId = this.EntityManager.EntityExists(this._nukieShuttle) ? new MapId?(base.Transform(this._nukieShuttle.Value).MapID) : null;
			MapId? targetStationMap = null;
			StationDataComponent data;
			if (this._targetStation != null && base.TryComp<StationDataComponent>(this._targetStation, ref data))
			{
				EntityUid? grid = Extensions.FirstOrNull<EntityUid>(data.Grids);
				targetStationMap = ((grid != null) ? new MapId?(base.Transform(grid.Value).MapID) : null);
			}
			if ((from ent in base.EntityQuery<NukeOperativeComponent, MobStateComponent, TransformComponent>(true)
			where ent.Item3.MapID == shuttleMapId || ent.Item3.MapID == targetStationMap
			select ent).Any((ValueTuple<NukeOperativeComponent, MobStateComponent, TransformComponent> ent) => ent.Item2.CurrentState == MobState.Alive && ent.Item1.Running))
			{
				return;
			}
			bool spawnsAvailable = base.EntityQuery<NukeOperativeSpawnerComponent>(true).Any<NukeOperativeSpawnerComponent>();
			if (spawnsAvailable && shuttleMapId == this._nukiePlanet)
			{
				return;
			}
			if (spawnsAvailable)
			{
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.NukiesAbandoned);
			}
			else
			{
				this._winConditions.Add(NukeopsRuleSystem.WinCondition.AllNukiesDead);
			}
			this.RuleWinType = NukeopsRuleSystem.WinType.CrewMajor;
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x00080BF8 File Offset: 0x0007EDF8
		private void OnNukeDisarm(NukeDisarmSuccessEvent ev)
		{
			this.CheckRoundShouldEnd();
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x00080C00 File Offset: 0x0007EE00
		private void OnMobStateChanged(EntityUid uid, NukeOperativeComponent component, MobStateChangedEvent ev)
		{
			if (ev.NewMobState == MobState.Dead)
			{
				this.CheckRoundShouldEnd();
			}
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x00080C14 File Offset: 0x0007EE14
		private void OnPlayersSpawning(RulePlayerSpawningEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			int playersPerOperative = this._nukeopsRuleConfig.PlayersPerOperative;
			int maxOperatives = this._nukeopsRuleConfig.MaxOperatives;
			List<IPlayerSession> everyone = new List<IPlayerSession>(ev.PlayerPool);
			List<IPlayerSession> prefList = new List<IPlayerSession>();
			List<IPlayerSession> cmdrPrefList = new List<IPlayerSession>();
			List<IPlayerSession> operatives = new List<IPlayerSession>();
			foreach (IPlayerSession player in everyone)
			{
				if (ev.Profiles.ContainsKey(player.UserId))
				{
					HumanoidCharacterProfile humanoidCharacterProfile = ev.Profiles[player.UserId];
					if (humanoidCharacterProfile.AntagPreferences.Contains(this._nukeopsRuleConfig.OperativeRoleProto))
					{
						prefList.Add(player);
					}
					if (humanoidCharacterProfile.AntagPreferences.Contains(this._nukeopsRuleConfig.CommanderRolePrototype))
					{
						cmdrPrefList.Add(player);
					}
				}
			}
			int numNukies = MathHelper.Clamp(ev.PlayerPool.Count / playersPerOperative, 1, maxOperatives);
			for (int i = 0; i < numNukies; i++)
			{
				IPlayerSession nukeOp;
				if (i == 0)
				{
					if (cmdrPrefList.Count == 0)
					{
						if (prefList.Count == 0)
						{
							if (everyone.Count == 0)
							{
								Logger.InfoS("preset", "Insufficient ready players to fill up with nukeops, stopping the selection");
								break;
							}
							nukeOp = RandomExtensions.PickAndTake<IPlayerSession>(this._random, everyone);
							Logger.InfoS("preset", "Insufficient preferred nukeop commanders or nukies, picking at random.");
						}
						else
						{
							nukeOp = RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList);
							everyone.Remove(nukeOp);
							Logger.InfoS("preset", "Insufficient preferred nukeop commanders, picking at random from regular op list.");
						}
					}
					else
					{
						nukeOp = RandomExtensions.PickAndTake<IPlayerSession>(this._random, cmdrPrefList);
						everyone.Remove(nukeOp);
						prefList.Remove(nukeOp);
						Logger.InfoS("preset", "Selected a preferred nukeop commander.");
					}
				}
				else if (prefList.Count == 0)
				{
					if (everyone.Count == 0)
					{
						Logger.InfoS("preset", "Insufficient ready players to fill up with nukeops, stopping the selection");
						break;
					}
					nukeOp = RandomExtensions.PickAndTake<IPlayerSession>(this._random, everyone);
					Logger.InfoS("preset", "Insufficient preferred nukeops, picking at random.");
				}
				else
				{
					nukeOp = RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList);
					everyone.Remove(nukeOp);
					Logger.InfoS("preset", "Selected a preferred nukeop.");
				}
				operatives.Add(nukeOp);
			}
			this.SpawnOperatives(numNukies, operatives, false);
			foreach (IPlayerSession session in operatives)
			{
				ev.PlayerPool.Remove(session);
				this.GameTicker.PlayerJoinGame(session);
				string name = (session.AttachedEntity == null) ? string.Empty : base.MetaData(session.AttachedEntity.Value).EntityName;
				this._operativePlayers[name] = session;
			}
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x00080EF8 File Offset: 0x0007F0F8
		private void OnPlayersGhostSpawning(EntityUid uid, NukeOperativeComponent component, GhostRoleSpawnerUsedEvent args)
		{
			EntityUid spawner = args.Spawner;
			NukeOperativeSpawnerComponent nukeOpSpawner;
			if (!base.TryComp<NukeOperativeSpawnerComponent>(spawner, ref nukeOpSpawner))
			{
				return;
			}
			HumanoidCharacterProfile profile = null;
			ActorComponent actor;
			if (base.TryComp<ActorComponent>(args.Spawned, ref actor))
			{
				profile = (this._prefs.GetPreferences(actor.PlayerSession.UserId).SelectedCharacter as HumanoidCharacterProfile);
			}
			this.SetupOperativeEntity(uid, nukeOpSpawner.OperativeName, nukeOpSpawner.OperativeStartingGear, profile);
			this._operativeMindPendingData.Add(uid, nukeOpSpawner.OperativeRolePrototype);
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x00080F74 File Offset: 0x0007F174
		private void OnMindAdded(EntityUid uid, NukeOperativeComponent component, MindAddedMessage args)
		{
			MindComponent mindComponent;
			if (!base.TryComp<MindComponent>(uid, ref mindComponent) || mindComponent.Mind == null)
			{
				return;
			}
			Mind mind = mindComponent.Mind;
			string role;
			if (this._operativeMindPendingData.TryGetValue(uid, out role))
			{
				mind.AddRole(new TraitorRole(mind, this._prototypeManager.Index<AntagPrototype>(role)));
				this._operativeMindPendingData.Remove(uid);
			}
			IPlayerSession playerSession;
			if (!mind.TryGetSession(out playerSession))
			{
				return;
			}
			if (this._operativePlayers.ContainsValue(playerSession))
			{
				return;
			}
			string name = base.MetaData(uid).EntityName;
			this._operativePlayers.Add(name, playerSession);
			if (this._ticker.RunLevel != GameRunLevel.InRound)
			{
				return;
			}
			if (this._nukeopsRuleConfig.GreetSound != null)
			{
				this._audioSystem.PlayGlobal(this._nukeopsRuleConfig.GreetSound, playerSession, null);
			}
			if (this._targetStation != null && !string.IsNullOrEmpty(base.Name(this._targetStation.Value, null)))
			{
				this._chatManager.DispatchServerMessage(playerSession, Loc.GetString("nukeops-welcome", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("station", this._targetStation.Value)
				}), false);
			}
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x000810AC File Offset: 0x0007F2AC
		private bool SpawnMap()
		{
			if (this._nukiePlanet != null)
			{
				return true;
			}
			ResourcePath path = this._nukeopsRuleConfig.NukieOutpostMap;
			ResourcePath shuttlePath = this._nukeopsRuleConfig.NukieShuttleMap;
			if (path == null)
			{
				Logger.ErrorS("nukies", "No station map specified for nukeops!");
				return false;
			}
			if (shuttlePath == null)
			{
				Logger.ErrorS("nukies", "No shuttle map specified for nukeops!");
				return false;
			}
			MapId mapId = this._mapManager.CreateMap(null);
			MapLoadOptions options = new MapLoadOptions
			{
				LoadMap = true
			};
			IReadOnlyList<EntityUid> outpostGrids;
			if (!this._map.TryLoad(mapId, path.ToString(), ref outpostGrids, options) || outpostGrids.Count == 0)
			{
				string text = "nukies";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Error loading map ");
				defaultInterpolatedStringHandler.AppendFormatted<ResourcePath>(path);
				defaultInterpolatedStringHandler.AppendLiteral(" for nukies!");
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			this._nukieOutpost = new EntityUid?(outpostGrids[0]);
			IReadOnlyList<EntityUid> grids;
			if (!this._map.TryLoad(mapId, shuttlePath.ToString(), ref grids, new MapLoadOptions
			{
				Offset = Vector2.One * 1000f
			}) || !grids.Any<EntityUid>())
			{
				string text2 = "nukies";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Error loading grid ");
				defaultInterpolatedStringHandler.AppendFormatted<ResourcePath>(shuttlePath);
				defaultInterpolatedStringHandler.AppendLiteral(" for nukies!");
				Logger.ErrorS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			EntityUid shuttleId = grids.First<EntityUid>();
			if (base.Deleted(shuttleId, null))
			{
				Logger.ErrorS("nukeops", "Tried to load nukeops shuttle as a map, aborting.");
				this._mapManager.DeleteMap(mapId);
				return false;
			}
			ShuttleComponent shuttle;
			if (base.TryComp<ShuttleComponent>(shuttleId, ref shuttle))
			{
				this._shuttleSystem.TryFTLDock(shuttle, this._nukieOutpost.Value);
			}
			this._nukiePlanet = new MapId?(mapId);
			this._nukieShuttle = new EntityUid?(shuttleId);
			return true;
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x00081290 File Offset: 0x0007F490
		[return: TupleElementNames(new string[]
		{
			"Name",
			"Role",
			"Gear"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			1,
			1
		})]
		private ValueTuple<string, string, string> GetOperativeSpawnDetails(int spawnNumber)
		{
			string name;
			string role;
			string gear;
			if (spawnNumber != 0)
			{
				if (spawnNumber != 1)
				{
					name = Loc.GetString("nukeops-role-operator") + " " + RandomExtensions.PickAndTake<string>(this._random, this._operativeNames[this._nukeopsRuleConfig.NormalNames]);
					role = this._nukeopsRuleConfig.OperativeRoleProto;
					gear = this._nukeopsRuleConfig.OperativeStartGearPrototype;
				}
				else
				{
					name = Loc.GetString("nukeops-role-agent") + " " + RandomExtensions.PickAndTake<string>(this._random, this._operativeNames[this._nukeopsRuleConfig.NormalNames]);
					role = this._nukeopsRuleConfig.OperativeRoleProto;
					gear = this._nukeopsRuleConfig.MedicStartGearPrototype;
				}
			}
			else
			{
				name = Loc.GetString("nukeops-role-commander") + " " + RandomExtensions.PickAndTake<string>(this._random, this._operativeNames[this._nukeopsRuleConfig.EliteNames]);
				role = this._nukeopsRuleConfig.CommanderRolePrototype;
				gear = this._nukeopsRuleConfig.CommanderStartGearPrototype;
			}
			return new ValueTuple<string, string, string>(name, role, gear);
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x000813A4 File Offset: 0x0007F5A4
		private void SetupOperativeEntity(EntityUid mob, string name, string gear, [Nullable(2)] HumanoidCharacterProfile profile)
		{
			base.MetaData(mob).EntityName = name;
			this.EntityManager.EnsureComponent<RandomHumanoidAppearanceComponent>(mob);
			this.EntityManager.EnsureComponent<NukeOperativeComponent>(mob);
			StartingGearPrototype gearPrototype;
			if (this._startingGearPrototypes.TryGetValue(gear, out gearPrototype))
			{
				this._stationSpawningSystem.EquipStartingGear(mob, gearPrototype, profile);
			}
			this._faction.RemoveFaction(mob, "NanoTrasen", false);
			this._faction.AddFaction(mob, "Syndicate", true);
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x0008141C File Offset: 0x0007F61C
		private void SpawnOperatives(int spawnCount, List<IPlayerSession> sessions, bool addSpawnPoints)
		{
			if (this._nukieOutpost == null)
			{
				return;
			}
			EntityUid outpostUid = this._nukieOutpost.Value;
			List<EntityCoordinates> spawns = new List<EntityCoordinates>();
			foreach (ValueTuple<SpawnPointComponent, MetaDataComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<SpawnPointComponent, MetaDataComponent, TransformComponent>(true))
			{
				MetaDataComponent meta = valueTuple.Item2;
				TransformComponent xform = valueTuple.Item3;
				EntityPrototype entityPrototype = meta.EntityPrototype;
				if (!(((entityPrototype != null) ? entityPrototype.ID : null) != this._nukeopsRuleConfig.SpawnPointPrototype) && !(xform.ParentUid != this._nukieOutpost))
				{
					spawns.Add(xform.Coordinates);
					break;
				}
			}
			if (spawns.Count == 0)
			{
				spawns.Add(this.EntityManager.GetComponent<TransformComponent>(outpostUid).Coordinates);
				Logger.WarningS("nukies", "Fell back to default spawn for nukies!");
			}
			for (int i = 0; i < spawnCount; i++)
			{
				ValueTuple<string, string, string> spawnDetails = this.GetOperativeSpawnDetails(i);
				AntagPrototype nukeOpsAntag = this._prototypeManager.Index<AntagPrototype>(spawnDetails.Item2);
				IPlayerSession session;
				if (Extensions.TryGetValue<IPlayerSession>(sessions, i, ref session))
				{
					EntityUid mob = this._randomHumanoid.SpawnRandomHumanoid(this._nukeopsRuleConfig.RandomHumanoidSettingsPrototype, RandomExtensions.Pick<EntityCoordinates>(this._random, spawns), string.Empty);
					HumanoidCharacterProfile profile = this._prefs.GetPreferences(session.UserId).SelectedCharacter as HumanoidCharacterProfile;
					this.SetupOperativeEntity(mob, spawnDetails.Item1, spawnDetails.Item3, profile);
					Mind mind = new Mind(session.UserId);
					mind.CharacterName = spawnDetails.Item1;
					mind.ChangeOwningPlayer(new NetUserId?(session.UserId));
					mind.AddRole(new TraitorRole(mind, nukeOpsAntag));
					mind.TransferTo(new EntityUid?(mob), false, false);
				}
				else if (addSpawnPoints)
				{
					EntityUid spawnPoint = this.EntityManager.SpawnEntity(this._nukeopsRuleConfig.GhostSpawnPointProto, RandomExtensions.Pick<EntityCoordinates>(this._random, spawns));
					GhostRoleMobSpawnerComponent ghostRoleMobSpawnerComponent = base.EnsureComp<GhostRoleMobSpawnerComponent>(spawnPoint);
					ghostRoleMobSpawnerComponent.RoleName = Loc.GetString(nukeOpsAntag.Name);
					ghostRoleMobSpawnerComponent.RoleDescription = Loc.GetString(nukeOpsAntag.Objective);
					NukeOperativeSpawnerComponent nukeOperativeSpawnerComponent = base.EnsureComp<NukeOperativeSpawnerComponent>(spawnPoint);
					nukeOperativeSpawnerComponent.OperativeName = spawnDetails.Item1;
					nukeOperativeSpawnerComponent.OperativeRolePrototype = spawnDetails.Item2;
					nukeOperativeSpawnerComponent.OperativeStartingGear = spawnDetails.Item3;
				}
			}
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x00081694 File Offset: 0x0007F894
		private void SpawnOperativesForGhostRoles()
		{
			int playersPerOperative = this._nukeopsRuleConfig.PlayersPerOperative;
			int maxOperatives = this._nukeopsRuleConfig.MaxOperatives;
			int numNukies = MathHelper.Clamp(this._playerSystem.ServerSessions.ToList<IPlayerSession>().Count / playersPerOperative, 1, maxOperatives);
			List<IPlayerSession> operatives = new List<IPlayerSession>();
			this.SpawnOperatives(numNukies, operatives, true);
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x000816E8 File Offset: 0x0007F8E8
		public void MakeLoneNukie(Mind mind)
		{
			if (mind.OwnedEntity == null)
			{
				return;
			}
			mind.AddRole(new TraitorRole(mind, this._prototypeManager.Index<AntagPrototype>(this._nukeopsRuleConfig.OperativeRoleProto)));
			base.AddComp<NukeOperativeComponent>(mind.OwnedEntity.Value);
			this._faction.RemoveFaction(mind.OwnedEntity.Value, "NanoTrasen", false);
			this._faction.AddFaction(mind.OwnedEntity.Value, "Syndicate", true);
			SetOutfitCommand.SetOutfit(mind.OwnedEntity.Value, "SyndicateOperativeGearFull", this.EntityManager, null);
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x0008179C File Offset: 0x0007F99C
		private void OnStartAttempt(RoundStartAttemptEvent ev)
		{
			if (base.RuleAdded)
			{
				NukeopsRuleConfiguration nukeOpsConfig = this.Configuration as NukeopsRuleConfiguration;
				if (nukeOpsConfig != null)
				{
					this._nukeopsRuleConfig = nukeOpsConfig;
					int? minPlayers = this._prototypeManager.Index<GamePresetPrototype>(this.Prototype).MinPlayers;
					if (!ev.Forced)
					{
						int num = ev.Players.Length;
						int? num2 = minPlayers;
						if (num < num2.GetValueOrDefault() & num2 != null)
						{
							this._chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-not-enough-ready-players", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("readyPlayersCount", ev.Players.Length),
								new ValueTuple<string, object>("minimumPlayers", minPlayers)
							}), null);
							ev.Cancel();
							return;
						}
					}
					if (ev.Players.Length != 0)
					{
						return;
					}
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("nukeops-no-one-ready"), null);
					ev.Cancel();
					return;
				}
			}
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x00081894 File Offset: 0x0007FA94
		public override void Started()
		{
			this.RuleWinType = NukeopsRuleSystem.WinType.Neutral;
			this._winConditions.Clear();
			this._nukieOutpost = null;
			this._nukiePlanet = null;
			this._startingGearPrototypes.Clear();
			this._operativeNames.Clear();
			this._operativeMindPendingData.Clear();
			this._operativePlayers.Clear();
			foreach (string proto in new string[]
			{
				this._nukeopsRuleConfig.CommanderStartGearPrototype,
				this._nukeopsRuleConfig.MedicStartGearPrototype,
				this._nukeopsRuleConfig.OperativeStartGearPrototype
			})
			{
				this._startingGearPrototypes.Add(proto, this._prototypeManager.Index<StartingGearPrototype>(proto));
			}
			foreach (string proto2 in new string[]
			{
				this._nukeopsRuleConfig.EliteNames,
				this._nukeopsRuleConfig.NormalNames
			})
			{
				this._operativeNames.Add(proto2, new List<string>(this._prototypeManager.Index<DatasetPrototype>(proto2).Values));
			}
			if (!this.SpawnMap())
			{
				Logger.InfoS("nukies", "Failed to load map for nukeops");
				return;
			}
			foreach (ValueTuple<NukeOperativeComponent, MindComponent> valueTuple in base.EntityQuery<NukeOperativeComponent, MindComponent>(true))
			{
				MindComponent mindComp = valueTuple.Item2;
				IPlayerSession session;
				if (mindComp.Mind != null && mindComp.Mind.TryGetSession(out session))
				{
					string name = base.MetaData(mindComp.Owner).EntityName;
					this._operativePlayers.Add(name, session);
				}
			}
			if (this.GameTicker.RunLevel == GameRunLevel.InRound)
			{
				this.SpawnOperativesForGhostRoles();
			}
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x00081A58 File Offset: 0x0007FC58
		public override void Ended()
		{
		}

		// Token: 0x04000F62 RID: 3938
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F63 RID: 3939
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000F64 RID: 3940
		[Dependency]
		private readonly IServerPreferencesManager _prefs;

		// Token: 0x04000F65 RID: 3941
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F66 RID: 3942
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000F67 RID: 3943
		[Dependency]
		private readonly IPlayerManager _playerSystem;

		// Token: 0x04000F68 RID: 3944
		[Dependency]
		private readonly FactionSystem _faction;

		// Token: 0x04000F69 RID: 3945
		[Dependency]
		private readonly StationSpawningSystem _stationSpawningSystem;

		// Token: 0x04000F6A RID: 3946
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000F6B RID: 3947
		[Dependency]
		private readonly ShuttleSystem _shuttleSystem;

		// Token: 0x04000F6C RID: 3948
		[Dependency]
		private readonly RoundEndSystem _roundEndSystem;

		// Token: 0x04000F6D RID: 3949
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000F6E RID: 3950
		[Dependency]
		private readonly GameTicker _ticker;

		// Token: 0x04000F6F RID: 3951
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x04000F70 RID: 3952
		[Dependency]
		private readonly RandomHumanoidSystem _randomHumanoid;

		// Token: 0x04000F71 RID: 3953
		private NukeopsRuleSystem.WinType _winType = NukeopsRuleSystem.WinType.Neutral;

		// Token: 0x04000F72 RID: 3954
		private List<NukeopsRuleSystem.WinCondition> _winConditions = new List<NukeopsRuleSystem.WinCondition>();

		// Token: 0x04000F73 RID: 3955
		private MapId? _nukiePlanet;

		// Token: 0x04000F74 RID: 3956
		private EntityUid? _nukieOutpost;

		// Token: 0x04000F75 RID: 3957
		private EntityUid? _nukieShuttle;

		// Token: 0x04000F76 RID: 3958
		private EntityUid? _targetStation;

		// Token: 0x04000F77 RID: 3959
		private NukeopsRuleConfiguration _nukeopsRuleConfig = new NukeopsRuleConfiguration();

		// Token: 0x04000F78 RID: 3960
		private readonly Dictionary<string, StartingGearPrototype> _startingGearPrototypes = new Dictionary<string, StartingGearPrototype>();

		// Token: 0x04000F79 RID: 3961
		private readonly Dictionary<string, List<string>> _operativeNames = new Dictionary<string, List<string>>();

		// Token: 0x04000F7A RID: 3962
		private readonly Dictionary<EntityUid, string> _operativeMindPendingData = new Dictionary<EntityUid, string>();

		// Token: 0x04000F7B RID: 3963
		private readonly Dictionary<string, IPlayerSession> _operativePlayers = new Dictionary<string, IPlayerSession>();

		// Token: 0x020009E3 RID: 2531
		[NullableContext(0)]
		private enum WinType
		{
			// Token: 0x04002285 RID: 8837
			OpsMajor,
			// Token: 0x04002286 RID: 8838
			OpsMinor,
			// Token: 0x04002287 RID: 8839
			Neutral,
			// Token: 0x04002288 RID: 8840
			CrewMinor,
			// Token: 0x04002289 RID: 8841
			CrewMajor
		}

		// Token: 0x020009E4 RID: 2532
		[NullableContext(0)]
		private enum WinCondition
		{
			// Token: 0x0400228B RID: 8843
			NukeExplodedOnCorrectStation,
			// Token: 0x0400228C RID: 8844
			NukeExplodedOnNukieOutpost,
			// Token: 0x0400228D RID: 8845
			NukeExplodedOnIncorrectLocation,
			// Token: 0x0400228E RID: 8846
			NukeActiveInStation,
			// Token: 0x0400228F RID: 8847
			NukeActiveAtCentCom,
			// Token: 0x04002290 RID: 8848
			NukeDiskOnCentCom,
			// Token: 0x04002291 RID: 8849
			NukeDiskNotOnCentCom,
			// Token: 0x04002292 RID: 8850
			NukiesAbandoned,
			// Token: 0x04002293 RID: 8851
			AllNukiesDead,
			// Token: 0x04002294 RID: 8852
			SomeNukiesAlive,
			// Token: 0x04002295 RID: 8853
			AllNukiesAlive
		}
	}
}
