using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Borgs;
using Content.Server.Chat.Managers;
using Content.Server.Flash;
using Content.Server.GameTicking.Presets;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.GameTicking.Rules.Configurations;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.NPC.Systems;
using Content.Server.Players;
using Content.Server.Preferences.Managers;
using Content.Server.Roles;
using Content.Server.RoundEnd;
using Content.Server.Station.Systems;
using Content.Server.Traitor;
using Content.Server.Traits.Assorted;
using Content.Server.White.Mindshield;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.White.Mindshield;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004BE RID: 1214
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevolutionRuleSystem : GameRuleSystem
	{
		// Token: 0x17000389 RID: 905
		// (get) Token: 0x060018E9 RID: 6377 RVA: 0x00082374 File Offset: 0x00080574
		// (set) Token: 0x060018EA RID: 6378 RVA: 0x0008237C File Offset: 0x0008057C
		private RevolutionRuleSystem.WinType RuleWinType
		{
			get
			{
				return this._winType;
			}
			set
			{
				this._winType = value;
				if (value == RevolutionRuleSystem.WinType.CrewMajor || value == RevolutionRuleSystem.WinType.RevMajor)
				{
					this._roundEndSystem.EndRound();
				}
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x060018EB RID: 6379 RVA: 0x00082397 File Offset: 0x00080597
		public override string Prototype
		{
			get
			{
				return "Revolution";
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x060018EC RID: 6380 RVA: 0x0008239E File Offset: 0x0008059E
		public int TotalHeadRevs
		{
			get
			{
				return this._revPlayers.Count;
			}
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x000823AC File Offset: 0x000805AC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundStartAttemptEvent>(new EntityEventHandler<RoundStartAttemptEvent>(this.OnStartAttempt), null, null);
			base.SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(new EntityEventHandler<RulePlayerJobsAssignedEvent>(this.OnPlayersSpawned), null, null);
			base.SubscribeLocalEvent<MindComponent, MobStateChangedEvent>(new ComponentEventHandler<MindComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndText), null, null);
			base.SubscribeLocalEvent<GameRunLevelChangedEvent>(new EntityEventHandler<GameRunLevelChangedEvent>(this.OnRunLevelChanged), null, null);
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentInit>(new ComponentEventHandler<RevolutionaryComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentRemove>(new ComponentEventHandler<RevolutionaryComponent, ComponentRemove>(this.OnComponentRemove), null, null);
			base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.HandleLatejoin), null, null);
			base.SubscribeLocalEvent<FlashAttemptEvent>(new EntityEventHandler<FlashAttemptEvent>(this.OnFlashAttempt), null, null);
			base.SubscribeLocalEvent<RevolutionaryComponent, ComponentGetState>(new ComponentEventRefHandler<RevolutionaryComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<MindShieldImplanted>(new EntityEventHandler<MindShieldImplanted>(this.OnMindshieldImplanted), null, null);
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0008249C File Offset: 0x0008069C
		private void OnMindshieldImplanted(MindShieldImplanted ev)
		{
			RevolutionaryComponent revolutionaryComponent;
			if (!base.TryComp<RevolutionaryComponent>(ev.Target, ref revolutionaryComponent))
			{
				return;
			}
			if (revolutionaryComponent.HeadRevolutionary)
			{
				this._mindShieldSystem.RemoveMindShieldImplant(ev.Target, ev.MindShield, true);
				return;
			}
			base.RemComp(ev.Target, revolutionaryComponent);
			ActorComponent actorComponent;
			if (!base.TryComp<ActorComponent>(ev.Target, ref actorComponent))
			{
				return;
			}
			this._chatManager.DispatchServerMessage(actorComponent.PlayerSession, "Ваш разум очистился, вы больше не революционер", false);
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x00082510 File Offset: 0x00080710
		private void OnFlashAttempt(FlashAttemptEvent msg)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (msg.User == null)
			{
				return;
			}
			if (msg.MassFlash)
			{
				return;
			}
			if (msg.Cancelled)
			{
				return;
			}
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(msg.Target, ref actor))
			{
				return;
			}
			if (base.HasComp<BorgComponent>(msg.Target))
			{
				return;
			}
			RevolutionaryComponent comp;
			if (!base.TryComp<RevolutionaryComponent>(msg.User, ref comp))
			{
				return;
			}
			if (!comp.HeadRevolutionary)
			{
				return;
			}
			MindComponent mindComponent;
			if (base.TryComp<MindComponent>(msg.Target, ref mindComponent) && mindComponent.HasMind && this._headJobPrototypes.Contains(mindComponent.Mind.CurrentJob.Prototype))
			{
				return;
			}
			if (base.HasComp<MindShieldComponent>(msg.Target))
			{
				return;
			}
			if (base.HasComp<RevolutionaryComponent>(msg.Target))
			{
				return;
			}
			RevolutionaryComponent targetComp = base.EnsureComp<RevolutionaryComponent>(msg.Target);
			targetComp.HeadRevolutionary = false;
			base.Dirty(targetComp, null);
			this._chatManager.DispatchServerMessage(actor.PlayerSession, Loc.GetString("rev-welcome-rev"), false);
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x0008260C File Offset: 0x0008080C
		private void OnComponentInit(EntityUid uid, RevolutionaryComponent component, ComponentInit args)
		{
			MindComponent mindComponent;
			if (!base.TryComp<MindComponent>(uid, ref mindComponent) || !base.RuleAdded)
			{
				return;
			}
			if (!mindComponent.HasMind)
			{
				return;
			}
			Mind mind = mindComponent.Mind;
			IPlayerSession session = (mind != null) ? mind.Session : null;
			if (session != null)
			{
				this._revPlayers.Add(session);
			}
			mindComponent.Mind.AddRole(new TraitorRole(mindComponent.Mind, this._antagPrototype));
			this._faction.RemoveFaction(uid, "NanoTrasen", true);
			base.RemComp<PacifistComponent>(uid);
			base.RemComp<PacifiedComponent>(uid);
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x00082698 File Offset: 0x00080898
		private void OnComponentRemove(EntityUid uid, RevolutionaryComponent component, ComponentRemove args)
		{
			MindComponent comp;
			IPlayerSession playerSession;
			if (base.TryComp<MindComponent>(uid, ref comp) && comp.HasMind && comp.Mind.TryGetSession(out playerSession))
			{
				this._revPlayers.Remove(playerSession);
				if (comp.Mind.HasRole<TraitorRole>())
				{
					foreach (Role role in comp.Mind.AllRoles)
					{
						TraitorRole traitorRole = role as TraitorRole;
						if (traitorRole != null && traitorRole.Prototype.ID == this._antagPrototype.ID)
						{
							comp.Mind.RemoveRole(role);
						}
					}
				}
				this._faction.AddFaction(uid, "NanoTrasen", true);
			}
			this.CheckRoundShouldEnd();
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x00082778 File Offset: 0x00080978
		private void OnGetState(EntityUid uid, RevolutionaryComponent component, ref ComponentGetState args)
		{
			args.State = new RevolutionaryComponentState(component.HeadRevolutionary);
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x0008278B File Offset: 0x0008098B
		private void OnRunLevelChanged(GameRunLevelChangedEvent ev)
		{
			if (ev.New == GameRunLevel.InRound)
			{
				this.OnRoundStart();
			}
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0008279C File Offset: 0x0008099C
		private void OnRoundStart()
		{
			Filter filter = Filter.Empty();
			foreach (RevolutionaryComponent headrev in base.EntityQuery<RevolutionaryComponent>(false))
			{
				ActorComponent actor;
				if (base.TryComp<ActorComponent>(headrev.Owner, ref actor))
				{
					this._chatManager.DispatchServerMessage(actor.PlayerSession, Loc.GetString("rev-welcome-headrev"), false);
					filter.AddPlayer(actor.PlayerSession);
				}
			}
			this._audioSystem.PlayGlobal(this._revRuleConfig.GreetSound, filter, false, null);
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x00082848 File Offset: 0x00080A48
		private void OnRoundEndText(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			string winText = Loc.GetString("rev-" + this._winType.ToString().ToLower());
			ev.AddLine(winText);
			foreach (RevolutionRuleSystem.WinCondition cond in this._winConditions)
			{
				string text = Loc.GetString("rev-cond-" + cond.ToString().ToLower());
				ev.AddLine(text);
			}
			ev.AddLine(Loc.GetString("rev-list-revs-start"));
			foreach (IPlayerSession session in this._revPlayers)
			{
				string text2 = "rev-list";
				ValueTuple<string, object>[] array = new ValueTuple<string, object>[3];
				int num = 0;
				string item = "name";
				PlayerData playerData = session.ContentData();
				object item2;
				if (playerData == null)
				{
					item2 = null;
				}
				else
				{
					Mind mind = playerData.Mind;
					item2 = ((mind != null) ? mind.CharacterName : null);
				}
				array[num] = new ValueTuple<string, object>(item, item2);
				array[1] = new ValueTuple<string, object>("user", session.Name);
				int num2 = 2;
				string item3 = "headrev";
				PlayerData playerData2 = session.ContentData();
				EntityUid? entityUid;
				if (playerData2 == null)
				{
					entityUid = null;
				}
				else
				{
					Mind mind2 = playerData2.Mind;
					entityUid = ((mind2 != null) ? mind2.OwnedEntity : null);
				}
				RevolutionaryComponent revcomp = base.CompOrNull<RevolutionaryComponent>(entityUid);
				array[num2] = new ValueTuple<string, object>(item3, (revcomp != null && revcomp.HeadRevolutionary) ? Loc.GetString("rev-list-headrevbool") : "");
				string listing = Loc.GetString(text2, array);
				ev.AddLine(listing);
			}
			ev.AddLine(Loc.GetString("rev-list-heads-start"));
			foreach (IPlayerSession session2 in this._headPlayers)
			{
				string text3 = "rev-list";
				ValueTuple<string, object>[] array2 = new ValueTuple<string, object>[3];
				int num3 = 0;
				string item4 = "name";
				PlayerData playerData3 = session2.ContentData();
				object item5;
				if (playerData3 == null)
				{
					item5 = null;
				}
				else
				{
					Mind mind3 = playerData3.Mind;
					item5 = ((mind3 != null) ? mind3.CharacterName : null);
				}
				array2[num3] = new ValueTuple<string, object>(item4, item5);
				array2[1] = new ValueTuple<string, object>("user", session2.Name);
				array2[2] = new ValueTuple<string, object>("headrev", "");
				string listing = Loc.GetString(text3, array2);
				ev.AddLine(listing);
			}
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x00082AD8 File Offset: 0x00080CD8
		private void CheckRoundShouldEnd()
		{
			if (!base.RuleAdded || this.RuleWinType == RevolutionRuleSystem.WinType.CrewMajor || this.RuleWinType == RevolutionRuleSystem.WinType.RevMajor)
			{
				return;
			}
			bool headRevsAlive = (from rev in base.EntityQuery<RevolutionaryComponent, MobStateComponent, MindComponent>(true)
			where rev.Item1.HeadRevolutionary
			select rev).Any((ValueTuple<RevolutionaryComponent, MobStateComponent, MindComponent> ent) => ent.Item2.CurrentState == MobState.Alive && ent.Item1.Running && ent.Item3.Mind != null);
			bool headCrewAlive = base.EntityQuery<MindComponent, MobStateComponent>(true).Where(delegate(ValueTuple<MindComponent, MobStateComponent> crew)
			{
				List<JobPrototype> headJobPrototypes = this._headJobPrototypes;
				MindComponent item = crew.Item1;
				JobPrototype item2;
				if (item == null)
				{
					item2 = null;
				}
				else
				{
					Mind mind = item.Mind;
					if (mind == null)
					{
						item2 = null;
					}
					else
					{
						Job currentJob = mind.CurrentJob;
						item2 = ((currentJob != null) ? currentJob.Prototype : null);
					}
				}
				return headJobPrototypes.Contains(item2);
			}).Any(delegate(ValueTuple<MindComponent, MobStateComponent> ent)
			{
				if (ent.Item2.CurrentState == MobState.Alive)
				{
					MindComponent item = ent.Item1;
					return item != null && item.Running && item.Mind != null;
				}
				return false;
			});
			if (headRevsAlive && headCrewAlive)
			{
				return;
			}
			if (!headRevsAlive)
			{
				this._winConditions.Add(RevolutionRuleSystem.WinCondition.AllHeadRevsDead);
				this.RuleWinType = RevolutionRuleSystem.WinType.CrewMajor;
				return;
			}
			this._winConditions.Add(RevolutionRuleSystem.WinCondition.AllCrewHeadsDead);
			this.RuleWinType = RevolutionRuleSystem.WinType.RevMajor;
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x00082BBC File Offset: 0x00080DBC
		private void OnMobStateChanged(EntityUid uid, MindComponent component, MobStateChangedEvent ev)
		{
			if (ev.NewMobState == MobState.Dead)
			{
				this.CheckRoundShouldEnd();
			}
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x00082BD0 File Offset: 0x00080DD0
		private void OnPlayersSpawned(RulePlayerJobsAssignedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			List<IPlayerSession> players = new List<IPlayerSession>(ev.Players);
			List<IPlayerSession> nonCrewHeads = players.Where(delegate(IPlayerSession player)
			{
				PlayerData playerData = player.ContentData();
				JobPrototype jobPrototype2;
				if (playerData == null)
				{
					jobPrototype2 = null;
				}
				else
				{
					Mind mind = playerData.Mind;
					jobPrototype2 = ((mind != null) ? mind.CurrentJob.Prototype : null);
				}
				JobPrototype jobPrototype = jobPrototype2;
				if (jobPrototype != null)
				{
					List<JobPrototype> headJobPrototypes = this._headJobPrototypes;
					PlayerData playerData2 = player.ContentData();
					JobPrototype item;
					if (playerData2 == null)
					{
						item = null;
					}
					else
					{
						Mind mind2 = playerData2.Mind;
						if (mind2 == null)
						{
							item = null;
						}
						else
						{
							Job currentJob = mind2.CurrentJob;
							item = ((currentJob != null) ? currentJob.Prototype : null);
						}
					}
					if (!headJobPrototypes.Contains(item))
					{
						return jobPrototype.CanBeAntag;
					}
				}
				return false;
			}).ToList<IPlayerSession>();
			this._headPlayers.AddRange(players.Where(delegate(IPlayerSession player)
			{
				if (!nonCrewHeads.Contains(player))
				{
					List<JobPrototype> headJobPrototypes = this._headJobPrototypes;
					PlayerData playerData = player.ContentData();
					JobPrototype item;
					if (playerData == null)
					{
						item = null;
					}
					else
					{
						Mind mind = playerData.Mind;
						if (mind == null)
						{
							item = null;
						}
						else
						{
							Job currentJob = mind.CurrentJob;
							item = ((currentJob != null) ? currentJob.Prototype : null);
						}
					}
					return headJobPrototypes.Contains(item);
				}
				return false;
			}));
			if (nonCrewHeads.Count == 0 || this._headPlayers.Count == 0)
			{
				this._gameTicker.EndGameRule(this._prototypeManager.Index<GameRulePrototype>("Revolution"));
				this._gameTicker.AddGameRule(this._prototypeManager.Index<GameRulePrototype>("Traitor"));
				return;
			}
			List<IPlayerSession> prefList = new List<IPlayerSession>();
			foreach (IPlayerSession player2 in nonCrewHeads)
			{
				if (ev.Profiles[player2.UserId].AntagPreferences.Contains(this.Prototype))
				{
					prefList.Add(player2);
				}
			}
			if (prefList.Count == 0)
			{
				prefList = nonCrewHeads;
			}
			int numHeadRevs = MathHelper.Clamp(prefList.Count / this._revRuleConfig.PlayersPerHeadRev, 1, this._revRuleConfig.MaxHeadRev);
			List<IPlayerSession> headRevs = new List<IPlayerSession>();
			for (int i = 0; i < numHeadRevs; i++)
			{
				headRevs.Add(RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList));
			}
			foreach (IPlayerSession headRev in headRevs)
			{
				this.MakeHeadRevolution(headRev);
			}
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x00082D9C File Offset: 0x00080F9C
		private void MakeHeadRevolution(IPlayerSession headRev)
		{
			PlayerData playerData = headRev.Data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				return;
			}
			EntityUid? ownedEntity = mind.OwnedEntity;
			if (ownedEntity != null)
			{
				EntityUid entity = ownedEntity.GetValueOrDefault();
				HumanoidCharacterProfile profile = this._prefs.GetPreferences(headRev.UserId).SelectedCharacter as HumanoidCharacterProfile;
				this._stationSpawningSystem.EquipStartingGear(entity, this._prototypeManager.Index<StartingGearPrototype>("HeadRevolutionaryGear"), profile);
				this.EntityManager.EnsureComponent<RevolutionaryComponent>(entity);
				this._audioSystem.PlayGlobal(this._revRuleConfig.GreetSound, Filter.Empty().AddPlayer(headRev), false, null);
				return;
			}
			Logger.ErrorS("preset", "Mind picked for headrev did not have an attached entity.");
		}

		// Token: 0x060018FA RID: 6394 RVA: 0x00082E64 File Offset: 0x00081064
		private void OnStartAttempt(RoundStartAttemptEvent ev)
		{
			if (base.RuleAdded)
			{
				RevolutionaryGameRuleConfiguration revRuleConfig = this.Configuration as RevolutionaryGameRuleConfiguration;
				if (revRuleConfig != null)
				{
					this._revRuleConfig = revRuleConfig;
					int? minPlayers = this._prototypeManager.Index<GamePresetPrototype>(this.Prototype).MinPlayers;
					if (!ev.Forced)
					{
						int num = ev.Players.Length;
						int? num2 = minPlayers;
						if (num < num2.GetValueOrDefault() & num2 != null)
						{
							this._chatManager.DispatchServerAnnouncement(Loc.GetString("rev-not-enough-ready-players", new ValueTuple<string, object>[]
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
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("rev-no-one-ready"), null);
					ev.Cancel();
					return;
				}
			}
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x00082F5C File Offset: 0x0008115C
		private void HandleLatejoin(PlayerSpawnCompleteEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (this.TotalHeadRevs >= this._revRuleConfig.MaxHeadRev)
			{
				return;
			}
			if (!ev.LateJoin)
			{
				return;
			}
			if (!ev.Profile.AntagPreferences.Contains(this.Prototype))
			{
				return;
			}
			JobPrototype job;
			if (ev.JobId == null || !this._prototypeManager.TryIndex<JobPrototype>(ev.JobId, ref job))
			{
				return;
			}
			if (this._headJobPrototypes.Contains(job))
			{
				this._headPlayers.Add(ev.Player);
				return;
			}
			if (!job.CanBeAntag)
			{
				return;
			}
			int target = this._revRuleConfig.PlayersPerHeadRev * this.TotalHeadRevs + 1;
			float chance = 1f / (float)this._revRuleConfig.PlayersPerHeadRev;
			if (ev.JoinOrder < target)
			{
				chance /= (float)(target - ev.JoinOrder);
			}
			else
			{
				chance *= (float)(ev.JoinOrder + 1 - target);
			}
			if (chance > 1f)
			{
				chance = 1f;
			}
			if (RandomExtensions.Prob(this._random, chance))
			{
				this.MakeHeadRevolution(ev.Player);
				this._chatManager.DispatchServerMessage(ev.Player, Loc.GetString("rev-welcome-headrev"), false);
			}
		}

		// Token: 0x060018FC RID: 6396 RVA: 0x00083080 File Offset: 0x00081280
		public override void Started()
		{
			this.RuleWinType = RevolutionRuleSystem.WinType.Neutral;
			this._winConditions.Clear();
			this._revPlayers.Clear();
			this._headPlayers.Clear();
			this._headJobPrototypes = (from job in this._prototypeManager.EnumeratePrototypes<JobPrototype>()
			where job.Access.Contains("Command") || job.AccessGroups.Contains("AllAccess")
			select job).ToList<JobPrototype>();
			this._antagPrototype = this._prototypeManager.Index<AntagPrototype>(this.Prototype);
			foreach (ValueTuple<RevolutionaryComponent, MindComponent> valueTuple in base.EntityQuery<RevolutionaryComponent, MindComponent>(true))
			{
				MindComponent mindComp = valueTuple.Item2;
				IPlayerSession session;
				if (mindComp.Mind != null && mindComp.Mind.TryGetSession(out session))
				{
					this._revPlayers.Add(session);
				}
			}
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x00083168 File Offset: 0x00081368
		public override void Ended()
		{
		}

		// Token: 0x04000F8B RID: 3979
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000F8C RID: 3980
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000F8D RID: 3981
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000F8E RID: 3982
		[Dependency]
		private readonly FactionSystem _faction;

		// Token: 0x04000F8F RID: 3983
		[Dependency]
		private readonly RoundEndSystem _roundEndSystem;

		// Token: 0x04000F90 RID: 3984
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000F91 RID: 3985
		[Dependency]
		private readonly StationSpawningSystem _stationSpawningSystem;

		// Token: 0x04000F92 RID: 3986
		[Dependency]
		private readonly IServerPreferencesManager _prefs;

		// Token: 0x04000F93 RID: 3987
		[Dependency]
		private readonly MindShieldSystem _mindShieldSystem;

		// Token: 0x04000F94 RID: 3988
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000F95 RID: 3989
		private RevolutionRuleSystem.WinType _winType = RevolutionRuleSystem.WinType.Neutral;

		// Token: 0x04000F96 RID: 3990
		private List<RevolutionRuleSystem.WinCondition> _winConditions = new List<RevolutionRuleSystem.WinCondition>();

		// Token: 0x04000F97 RID: 3991
		private AntagPrototype _antagPrototype = new AntagPrototype();

		// Token: 0x04000F98 RID: 3992
		private RevolutionaryGameRuleConfiguration _revRuleConfig = new RevolutionaryGameRuleConfiguration();

		// Token: 0x04000F99 RID: 3993
		private readonly List<IPlayerSession> _revPlayers = new List<IPlayerSession>();

		// Token: 0x04000F9A RID: 3994
		private readonly List<IPlayerSession> _headPlayers = new List<IPlayerSession>();

		// Token: 0x04000F9B RID: 3995
		private List<JobPrototype> _headJobPrototypes = new List<JobPrototype>();

		// Token: 0x020009EA RID: 2538
		[NullableContext(0)]
		private enum WinType
		{
			// Token: 0x040022A2 RID: 8866
			RevMajor,
			// Token: 0x040022A3 RID: 8867
			Neutral,
			// Token: 0x040022A4 RID: 8868
			CrewMajor
		}

		// Token: 0x020009EB RID: 2539
		[NullableContext(0)]
		private enum WinCondition
		{
			// Token: 0x040022A6 RID: 8870
			AllHeadRevsDead,
			// Token: 0x040022A7 RID: 8871
			AllCrewHeadsDead
		}
	}
}
