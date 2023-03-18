using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Presets;
using Content.Server.Mind;
using Content.Server.NPC.Systems;
using Content.Server.Objectives;
using Content.Server.Objectives.Interfaces;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Server.Traitor;
using Content.Server.Traitor.Uplink;
using Content.Shared.CCVar;
using Content.Shared.Dataset;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.GameTicking.Rules
{
	// Token: 0x020004C3 RID: 1219
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TraitorRuleSystem : GameRuleSystem
	{
		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001932 RID: 6450 RVA: 0x0008487F File Offset: 0x00082A7F
		public override string Prototype
		{
			get
			{
				return "Traitor";
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001933 RID: 6451 RVA: 0x00084886 File Offset: 0x00082A86
		public int TotalTraitors
		{
			get
			{
				return this.Traitors.Count;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001934 RID: 6452 RVA: 0x00084893 File Offset: 0x00082A93
		private int _playersPerTraitor
		{
			get
			{
				return this._cfg.GetCVar<int>(CCVars.TraitorPlayersPerTraitor);
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001935 RID: 6453 RVA: 0x000848A5 File Offset: 0x00082AA5
		private int _maxTraitors
		{
			get
			{
				return this._cfg.GetCVar<int>(CCVars.TraitorMaxTraitors);
			}
		}

		// Token: 0x06001936 RID: 6454 RVA: 0x000848B8 File Offset: 0x00082AB8
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("preset");
			base.SubscribeLocalEvent<RoundStartAttemptEvent>(new EntityEventHandler<RoundStartAttemptEvent>(this.OnStartAttempt), null, null);
			base.SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(new EntityEventHandler<RulePlayerJobsAssignedEvent>(this.OnPlayersSpawned), null, null);
			base.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.HandleLatejoin), null, null);
			base.SubscribeLocalEvent<RoundEndTextAppendEvent>(new EntityEventHandler<RoundEndTextAppendEvent>(this.OnRoundEndText), null, null);
		}

		// Token: 0x06001937 RID: 6455 RVA: 0x0008492B File Offset: 0x00082B2B
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this.SelectionStatus == TraitorRuleSystem.SelectionState.ReadyToSelect && this._gameTiming.CurTime >= this._announceAt)
			{
				this.DoTraitorStart();
			}
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x0008495B File Offset: 0x00082B5B
		public override void Started()
		{
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x0008495D File Offset: 0x00082B5D
		public override void Ended()
		{
			this.Traitors.Clear();
			this._startCandidates.Clear();
			this.SelectionStatus = TraitorRuleSystem.SelectionState.WaitingForSpawn;
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x0008497C File Offset: 0x00082B7C
		private void OnStartAttempt(RoundStartAttemptEvent ev)
		{
			this.MakeCodewords();
			if (!base.RuleAdded)
			{
				return;
			}
			int? minPlayers = this._prototypeManager.Index<GamePresetPrototype>("Traitor").MinPlayers;
			if (!ev.Forced)
			{
				int num = ev.Players.Length;
				int? num2 = minPlayers;
				if (num < num2.GetValueOrDefault() & num2 != null)
				{
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("traitor-not-enough-ready-players", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("readyPlayersCount", ev.Players.Length),
						new ValueTuple<string, object>("minimumPlayers", minPlayers)
					}), null);
					ev.Cancel();
					return;
				}
			}
			if (ev.Players.Length == 0)
			{
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("traitor-no-one-ready"), null);
				ev.Cancel();
			}
		}

		// Token: 0x0600193B RID: 6459 RVA: 0x00084A60 File Offset: 0x00082C60
		private void MakeCodewords()
		{
			int cvar = this._cfg.GetCVar<int>(CCVars.TraitorCodewordCount);
			IEnumerable<string> values = this._prototypeManager.Index<DatasetPrototype>("adjectives").Values;
			IReadOnlyList<string> verbs = this._prototypeManager.Index<DatasetPrototype>("verbs").Values;
			List<string> codewordPool = values.Concat(verbs).ToList<string>();
			int finalCodewordCount = Math.Min(cvar, codewordPool.Count);
			this.Codewords = new string[finalCodewordCount];
			for (int i = 0; i < finalCodewordCount; i++)
			{
				this.Codewords[i] = RandomExtensions.PickAndTake<string>(this._random, codewordPool);
			}
		}

		// Token: 0x0600193C RID: 6460 RVA: 0x00084AEC File Offset: 0x00082CEC
		private void DoTraitorStart()
		{
			if (!this._startCandidates.Any<KeyValuePair<IPlayerSession, HumanoidCharacterProfile>>())
			{
				this._sawmill.Error("Tried to start Traitor mode without any candidates.");
				return;
			}
			int numTraitors = MathHelper.Clamp(this._startCandidates.Count / this._playersPerTraitor, 1, this._maxTraitors);
			this._cfg.GetCVar<int>(CCVars.TraitorCodewordCount);
			List<IPlayerSession> traitorPool = this.FindPotentialTraitors(this._startCandidates);
			foreach (IPlayerSession traitor in this.PickTraitors(numTraitors, traitorPool))
			{
				this.MakeTraitor(traitor);
			}
			this.SelectionStatus = TraitorRuleSystem.SelectionState.SelectionMade;
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00084BA4 File Offset: 0x00082DA4
		private void OnPlayersSpawned(RulePlayerJobsAssignedEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			foreach (IPlayerSession player in ev.Players)
			{
				if (ev.Profiles.ContainsKey(player.UserId))
				{
					this._startCandidates[player] = ev.Profiles[player.UserId];
				}
			}
			TimeSpan delay = TimeSpan.FromSeconds((double)(this._cfg.GetCVar<float>(CCVars.TraitorStartDelay) + this._random.NextFloat(0f, this._cfg.GetCVar<float>(CCVars.TraitorStartDelayVariance))));
			this._announceAt = this._gameTiming.CurTime + delay;
			this.SelectionStatus = TraitorRuleSystem.SelectionState.ReadyToSelect;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x00084C5C File Offset: 0x00082E5C
		public List<IPlayerSession> FindPotentialTraitors(in Dictionary<IPlayerSession, HumanoidCharacterProfile> candidates)
		{
			List<IPlayerSession> list = new List<IPlayerSession>(candidates.Keys).Where(delegate(IPlayerSession x)
			{
				PlayerData playerData = x.Data.ContentData();
				bool? flag;
				if (playerData == null)
				{
					flag = null;
				}
				else
				{
					Mind mind = playerData.Mind;
					if (mind == null)
					{
						flag = null;
					}
					else
					{
						flag = new bool?(mind.AllRoles.All(delegate(Role role)
						{
							Job job = role as Job;
							return job == null || job.CanBeAntag;
						}));
					}
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}).ToList<IPlayerSession>();
			List<IPlayerSession> prefList = new List<IPlayerSession>();
			foreach (IPlayerSession player in list)
			{
				if (candidates[player].AntagPreferences.Contains("Traitor"))
				{
					prefList.Add(player);
				}
			}
			if (prefList.Count == 0)
			{
				this._sawmill.Info("Insufficient preferred traitors, picking at random.");
				prefList = list;
			}
			return prefList;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x00084D1C File Offset: 0x00082F1C
		public List<IPlayerSession> PickTraitors(int traitorCount, List<IPlayerSession> prefList)
		{
			List<IPlayerSession> results = new List<IPlayerSession>(traitorCount);
			if (prefList.Count == 0)
			{
				this._sawmill.Info("Insufficient ready players to fill up with traitors, stopping the selection.");
				return results;
			}
			for (int i = 0; i < traitorCount; i++)
			{
				results.Add(RandomExtensions.PickAndTake<IPlayerSession>(this._random, prefList));
				this._sawmill.Info("Selected a preferred traitor.");
			}
			return results;
		}

		// Token: 0x06001940 RID: 6464 RVA: 0x00084D7C File Offset: 0x00082F7C
		public bool MakeTraitor(IPlayerSession traitor)
		{
			PlayerData playerData = traitor.Data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			if (mind == null)
			{
				this._sawmill.Info("Failed getting mind for picked traitor.");
				return false;
			}
			EntityUid? ownedEntity = mind.OwnedEntity;
			if (ownedEntity == null)
			{
				Logger.ErrorS("preset", "Mind picked for traitor did not have an attached entity.");
				return false;
			}
			EntityUid entity = ownedEntity.GetValueOrDefault();
			int startingBalance = this._cfg.GetCVar<int>(CCVars.TraitorStartingBalance);
			if (mind.CurrentJob != null)
			{
				startingBalance = Math.Max(startingBalance - mind.CurrentJob.Prototype.AntagAdvantage, 0);
			}
			if (!this._uplink.AddUplink(mind.OwnedEntity.Value, new FixedPoint2?(startingBalance), "StorePresetUplink", null))
			{
				return false;
			}
			AntagPrototype antagPrototype = this._prototypeManager.Index<AntagPrototype>("Traitor");
			TraitorRole traitorRole = new TraitorRole(mind, antagPrototype);
			mind.AddRole(traitorRole);
			this.Traitors.Add(traitorRole);
			traitorRole.GreetTraitor(this.Codewords);
			this._faction.RemoveFaction(entity, "NanoTrasen", false);
			this._faction.AddFaction(entity, "Syndicate", true);
			int maxDifficulty = this._cfg.GetCVar<int>(CCVars.TraitorMaxDifficulty);
			int maxPicks = this._cfg.GetCVar<int>(CCVars.TraitorMaxPicks);
			float difficulty = 0f;
			int pick = 0;
			while (pick < maxPicks && (float)maxDifficulty > difficulty)
			{
				ObjectivePrototype objective = this._objectivesManager.GetRandomObjective(traitorRole.Mind, "TraitorObjectiveGroups");
				if (objective != null && traitorRole.Mind.TryAddObjective(objective))
				{
					difficulty += objective.Difficulty;
				}
				pick++;
			}
			traitorRole.Mind.Briefing = Loc.GetString("traitor-role-codewords", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("codewords", string.Join(", ", this.Codewords))
			});
			this._audioSystem.PlayGlobal(this._addedSound, Filter.Empty().AddPlayer(traitor), false, new AudioParams?(AudioParams.Default));
			return true;
		}

		// Token: 0x06001941 RID: 6465 RVA: 0x00084F90 File Offset: 0x00083190
		private void HandleLatejoin(PlayerSpawnCompleteEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			if (this.TotalTraitors >= this._maxTraitors)
			{
				return;
			}
			if (!ev.LateJoin)
			{
				return;
			}
			if (!ev.Profile.AntagPreferences.Contains("Traitor"))
			{
				return;
			}
			JobPrototype job;
			if (ev.JobId == null || !this._prototypeManager.TryIndex<JobPrototype>(ev.JobId, ref job))
			{
				return;
			}
			if (!job.CanBeAntag)
			{
				return;
			}
			if (this.SelectionStatus < TraitorRuleSystem.SelectionState.SelectionMade)
			{
				this._startCandidates[ev.Player] = ev.Profile;
				return;
			}
			int target = this._playersPerTraitor * this.TotalTraitors + 1;
			float chance = 1f / (float)this._playersPerTraitor;
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
				this.MakeTraitor(ev.Player);
			}
		}

		// Token: 0x06001942 RID: 6466 RVA: 0x0008508C File Offset: 0x0008328C
		private void OnRoundEndText(RoundEndTextAppendEvent ev)
		{
			if (!base.RuleAdded)
			{
				return;
			}
			string result = Loc.GetString("traitor-round-end-result", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("traitorCount", this.Traitors.Count)
			});
			foreach (TraitorRole traitorRole in this.Traitors)
			{
				string name = traitorRole.Mind.CharacterName;
				IPlayerSession session;
				traitorRole.Mind.TryGetSession(out session);
				string username = (session != null) ? session.Name : null;
				Objective[] objectives = traitorRole.Mind.AllObjectives.ToArray<Objective>();
				if (objectives.Length == 0)
				{
					if (username != null)
					{
						if (name == null)
						{
							result = result + "\n" + Loc.GetString("traitor-user-was-a-traitor", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("user", username)
							});
						}
						else
						{
							result = result + "\n" + Loc.GetString("traitor-user-was-a-traitor-named", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("user", username),
								new ValueTuple<string, object>("name", name)
							});
						}
					}
					else if (name != null)
					{
						result = result + "\n" + Loc.GetString("traitor-was-a-traitor-named", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", name)
						});
					}
				}
				else
				{
					if (username != null)
					{
						if (name == null)
						{
							result = result + "\n" + Loc.GetString("traitor-user-was-a-traitor-with-objectives", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("user", username)
							});
						}
						else
						{
							result = result + "\n" + Loc.GetString("traitor-user-was-a-traitor-with-objectives-named", new ValueTuple<string, object>[]
							{
								new ValueTuple<string, object>("user", username),
								new ValueTuple<string, object>("name", name)
							});
						}
					}
					else if (name != null)
					{
						result = result + "\n" + Loc.GetString("traitor-was-a-traitor-with-objectives-named", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("name", name)
						});
					}
					foreach (IGrouping<string, Objective> objectiveGroup in from o in objectives
					group o by o.Prototype.Issuer)
					{
						result = result + "\n" + Loc.GetString("preset-traitor-objective-issuer-" + objectiveGroup.Key);
						foreach (Objective objective in objectiveGroup)
						{
							foreach (IObjectiveCondition condition in objective.Conditions)
							{
								float progress = condition.Progress;
								if (progress > 0.99f)
								{
									result = result + "\n- " + Loc.GetString("traitor-objective-condition-success", new ValueTuple<string, object>[]
									{
										new ValueTuple<string, object>("condition", condition.Title),
										new ValueTuple<string, object>("markupColor", "green")
									});
								}
								else
								{
									result = result + "\n- " + Loc.GetString("traitor-objective-condition-fail", new ValueTuple<string, object>[]
									{
										new ValueTuple<string, object>("condition", condition.Title),
										new ValueTuple<string, object>("progress", (int)(progress * 100f)),
										new ValueTuple<string, object>("markupColor", "red")
									});
								}
							}
						}
					}
				}
			}
			ev.AddLine(result);
		}

		// Token: 0x06001943 RID: 6467 RVA: 0x000854BC File Offset: 0x000836BC
		public IEnumerable<TraitorRole> GetOtherTraitorsAliveAndConnected(Mind ourMind)
		{
			List<TraitorRole> traitors = this.Traitors;
			new List<TraitorRole>();
			return from t in this.Traitors
			where t.Mind != null
			where t.Mind.OwnedEntity != null
			where t.Mind.Session != null
			where t.Mind != ourMind
			where this._mobStateSystem.IsAlive(t.Mind.OwnedEntity.Value, null)
			where t.Mind.CurrentEntity == t.Mind.OwnedEntity
			select t;
		}

		// Token: 0x04000FC4 RID: 4036
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000FC5 RID: 4037
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000FC6 RID: 4038
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000FC7 RID: 4039
		[Dependency]
		private readonly IObjectivesManager _objectivesManager;

		// Token: 0x04000FC8 RID: 4040
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000FC9 RID: 4041
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000FCA RID: 4042
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x04000FCB RID: 4043
		[Dependency]
		private readonly FactionSystem _faction;

		// Token: 0x04000FCC RID: 4044
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000FCD RID: 4045
		[Dependency]
		private readonly UplinkSystem _uplink;

		// Token: 0x04000FCE RID: 4046
		[Dependency]
		private readonly SharedAudioSystem _audioSystem;

		// Token: 0x04000FCF RID: 4047
		private ISawmill _sawmill;

		// Token: 0x04000FD0 RID: 4048
		private readonly SoundSpecifier _addedSound = new SoundPathSpecifier("/Audio/Misc/tatoralert.ogg", null);

		// Token: 0x04000FD1 RID: 4049
		public List<TraitorRole> Traitors = new List<TraitorRole>();

		// Token: 0x04000FD2 RID: 4050
		private const string TraitorPrototypeID = "Traitor";

		// Token: 0x04000FD3 RID: 4051
		private const string TraitorUplinkPresetId = "StorePresetUplink";

		// Token: 0x04000FD4 RID: 4052
		public string[] Codewords = new string[3];

		// Token: 0x04000FD5 RID: 4053
		public TraitorRuleSystem.SelectionState SelectionStatus;

		// Token: 0x04000FD6 RID: 4054
		private TimeSpan _announceAt = TimeSpan.Zero;

		// Token: 0x04000FD7 RID: 4055
		private Dictionary<IPlayerSession, HumanoidCharacterProfile> _startCandidates = new Dictionary<IPlayerSession, HumanoidCharacterProfile>();

		// Token: 0x020009F2 RID: 2546
		[NullableContext(0)]
		public enum SelectionState
		{
			// Token: 0x040022BB RID: 8891
			WaitingForSpawn,
			// Token: 0x040022BC RID: 8892
			ReadyToSelect,
			// Token: 0x040022BD RID: 8893
			SelectionMade
		}
	}
}
