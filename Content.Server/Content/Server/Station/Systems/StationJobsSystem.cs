using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.GameTicking;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Station.Components;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Station.Systems
{
	// Token: 0x02000198 RID: 408
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationJobsSystem : EntitySystem
	{
		// Token: 0x06000809 RID: 2057 RVA: 0x00028068 File Offset: 0x00026268
		public override void Initialize()
		{
			base.SubscribeLocalEvent<StationInitializedEvent>(new EntityEventHandler<StationInitializedEvent>(this.OnStationInitialized), null, null);
			base.SubscribeLocalEvent<StationJobsComponent, StationRenamedEvent>(new ComponentEventHandler<StationJobsComponent, StationRenamedEvent>(this.OnStationRenamed), null, null);
			base.SubscribeLocalEvent<StationJobsComponent, ComponentShutdown>(new ComponentEventHandler<StationJobsComponent, ComponentShutdown>(this.OnStationDeletion), null, null);
			base.SubscribeLocalEvent<PlayerJoinedLobbyEvent>(new EntityEventHandler<PlayerJoinedLobbyEvent>(this.OnPlayerJoinedLobby), null, null);
			this._configurationManager.OnValueChanged<bool>(CCVars.GameDisallowLateJoins, delegate(bool _)
			{
				this.UpdateJobsAvailable();
			}, true);
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x000280E2 File Offset: 0x000262E2
		public override void Update(float _)
		{
			if (this._availableJobsDirty)
			{
				this._cachedAvailableJobs = this.GenerateJobsAvailableEvent();
				base.RaiseNetworkEvent(this._cachedAvailableJobs, Filter.Empty().AddPlayers(this._playerManager.ServerSessions), true);
				this._availableJobsDirty = false;
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00028121 File Offset: 0x00026321
		private void OnStationDeletion(EntityUid uid, StationJobsComponent component, ComponentShutdown args)
		{
			this.UpdateJobsAvailable();
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0002812C File Offset: 0x0002632C
		private void OnStationInitialized(StationInitializedEvent msg)
		{
			StationJobsComponent stationJobs = base.AddComp<StationJobsComponent>(msg.Station);
			StationDataComponent stationData = base.Comp<StationDataComponent>(msg.Station);
			if (stationData.StationConfig == null)
			{
				return;
			}
			IReadOnlyDictionary<string, List<int?>> mapJobList = stationData.StationConfig.AvailableJobs;
			stationJobs.RoundStartTotalJobs = mapJobList.Values.Where(delegate(List<int?> x)
			{
				if (x[0] != null)
				{
					int? num = x[0];
					int num2 = 0;
					return num.GetValueOrDefault() > num2 & num != null;
				}
				return false;
			}).Sum((List<int?> x) => x[0].Value);
			stationJobs.MidRoundTotalJobs = mapJobList.Values.Where(delegate(List<int?> x)
			{
				if (x[1] != null)
				{
					int? num = x[1];
					int num2 = 0;
					return num.GetValueOrDefault() > num2 & num != null;
				}
				return false;
			}).Sum((List<int?> x) => x[1].Value);
			stationJobs.TotalJobs = stationJobs.MidRoundTotalJobs;
			stationJobs.JobList = mapJobList.ToDictionary((KeyValuePair<string, List<int?>> x) => x.Key, delegate(KeyValuePair<string, List<int?>> x)
			{
				int? num = x.Value[1];
				int num2 = -1;
				if (num.GetValueOrDefault() <= num2 & num != null)
				{
					return null;
				}
				num = x.Value[1];
				if (num == null)
				{
					return null;
				}
				return new uint?((uint)num.GetValueOrDefault());
			});
			stationJobs.RoundStartJobList = mapJobList.ToDictionary((KeyValuePair<string, List<int?>> x) => x.Key, delegate(KeyValuePair<string, List<int?>> x)
			{
				int? num = x.Value[0];
				int num2 = -1;
				if (num.GetValueOrDefault() <= num2 & num != null)
				{
					return null;
				}
				num = x.Value[0];
				if (num == null)
				{
					return null;
				}
				return new uint?((uint)num.GetValueOrDefault());
			});
			stationJobs.OverflowJobs = stationData.StationConfig.OverflowJobs.ToHashSet<string>();
			this.UpdateJobsAvailable();
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x000282CC File Offset: 0x000264CC
		public bool TryAssignJob(EntityUid station, JobPrototype job, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.TryAssignJob(station, job.ID, stationJobs);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x000282DC File Offset: 0x000264DC
		public bool TryAssignJob(EntityUid station, string jobPrototypeId, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.TryAdjustJobSlot(station, jobPrototypeId, -1, false, false, stationJobs);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x000282EA File Offset: 0x000264EA
		public bool TryAdjustJobSlot(EntityUid station, JobPrototype job, int amount, bool createSlot = false, bool clamp = false, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.TryAdjustJobSlot(station, job.ID, amount, createSlot, clamp, stationJobs);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00028300 File Offset: 0x00026500
		public bool TryAdjustJobSlot(EntityUid station, string jobPrototypeId, int amount, bool createSlot = false, bool clamp = false, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			Dictionary<string, uint?> jobList = stationJobs.JobList;
			if (!jobList.ContainsKey(jobPrototypeId))
			{
				if (amount < 0)
				{
					return false;
				}
				if (!createSlot)
				{
					return false;
				}
				stationJobs.TotalJobs += amount;
				jobList[jobPrototypeId] = new uint?((uint)amount);
				this.UpdateJobsAvailable();
				return true;
			}
			else
			{
				if (jobList[jobPrototypeId] == null)
				{
					return true;
				}
				if (amount < 0)
				{
					uint? num = jobList[jobPrototypeId];
					long? num2 = ((num != null) ? new long?((long)((ulong)num.GetValueOrDefault())) : null) + (long)amount;
					long num3 = 0L;
					if ((num2.GetValueOrDefault() < num3 & num2 != null) && !clamp)
					{
						return false;
					}
				}
				stationJobs.TotalJobs += amount;
				if (amount > 0)
				{
					Dictionary<string, uint?> dictionary = jobList;
					dictionary[jobPrototypeId] += (uint)amount;
				}
				else if (jobList[jobPrototypeId].Value - (uint)Math.Abs(amount) <= 0U)
				{
					jobList[jobPrototypeId] = new uint?(0U);
				}
				else
				{
					Dictionary<string, uint?> dictionary = jobList;
					dictionary[jobPrototypeId] -= (uint)Math.Abs(amount);
				}
				this.UpdateJobsAvailable();
				return true;
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x000284BC File Offset: 0x000266BC
		public bool TrySetJobSlot(EntityUid station, JobPrototype jobPrototype, int amount, bool createSlot = false, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.TrySetJobSlot(station, jobPrototype.ID, amount, createSlot, stationJobs);
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x000284D0 File Offset: 0x000266D0
		public bool TrySetJobSlot(EntityUid station, string jobPrototypeId, int amount, bool createSlot = false, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			if (amount < 0)
			{
				throw new ArgumentException("Tried to set a job to have a negative number of slots!", "amount");
			}
			Dictionary<string, uint?> jobList = stationJobs.JobList;
			if (jobList.ContainsKey(jobPrototypeId))
			{
				stationJobs.TotalJobs += amount - (int)jobList[jobPrototypeId].GetValueOrDefault();
				jobList[jobPrototypeId] = new uint?((uint)amount);
				this.UpdateJobsAvailable();
				return true;
			}
			if (!createSlot)
			{
				return false;
			}
			stationJobs.TotalJobs += amount;
			jobList[jobPrototypeId] = new uint?((uint)amount);
			this.UpdateJobsAvailable();
			return true;
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0002857B File Offset: 0x0002677B
		public void MakeJobUnlimited(EntityUid station, JobPrototype job, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			this.MakeJobUnlimited(station, job.ID, stationJobs);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0002858C File Offset: 0x0002678C
		public void MakeJobUnlimited(EntityUid station, string jobPrototypeId, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			if (stationJobs.JobList.ContainsKey(jobPrototypeId) && stationJobs.JobList[jobPrototypeId] != null)
			{
				stationJobs.TotalJobs -= (int)stationJobs.JobList[jobPrototypeId].Value;
			}
			stationJobs.JobList[jobPrototypeId] = null;
			this.UpdateJobsAvailable();
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00028615 File Offset: 0x00026815
		public bool IsJobUnlimited(EntityUid station, JobPrototype job, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.IsJobUnlimited(station, job.ID, stationJobs);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00028628 File Offset: 0x00026828
		public bool IsJobUnlimited(EntityUid station, string jobPrototypeId, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			uint? job;
			return stationJobs.JobList.TryGetValue(jobPrototypeId, out job) && job == null;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0002866D File Offset: 0x0002686D
		public bool TryGetJobSlot(EntityUid station, JobPrototype job, out uint? slots, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			return this.TryGetJobSlot(station, job.ID, out slots, stationJobs);
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00028680 File Offset: 0x00026880
		public bool TryGetJobSlot(EntityUid station, string jobPrototypeId, out uint? slots, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			uint? job;
			if (stationJobs.JobList.TryGetValue(jobPrototypeId, out job))
			{
				slots = job;
				return true;
			}
			slots = null;
			return false;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x000286CC File Offset: 0x000268CC
		public IReadOnlySet<string> GetAvailableJobs(EntityUid station, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			return (from x in stationJobs.JobList.Where(delegate(KeyValuePair<string, uint?> x)
			{
				uint? value = x.Value;
				uint num = 0U;
				return !(value.GetValueOrDefault() == num & value != null);
			})
			select x.Key).ToHashSet<string>();
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x00028748 File Offset: 0x00026948
		public IReadOnlySet<string> GetOverflowJobs(EntityUid station, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			return stationJobs.OverflowJobs.ToHashSet<string>();
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x00028771 File Offset: 0x00026971
		public IReadOnlyDictionary<string, uint?> GetJobs(EntityUid station, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			return stationJobs.JobList;
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00028795 File Offset: 0x00026995
		public IReadOnlyDictionary<string, uint?> GetRoundStartJobs(EntityUid station, [Nullable(2)] StationJobsComponent stationJobs = null)
		{
			if (!base.Resolve<StationJobsComponent>(station, ref stationJobs, true))
			{
				throw new ArgumentException("Tried to use a non-station entity as a station!", "station");
			}
			return stationJobs.RoundStartJobList;
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x000287BC File Offset: 0x000269BC
		[return: Nullable(2)]
		public string PickBestAvailableJobWithPriority(EntityUid station, IReadOnlyDictionary<string, JobPriority> jobPriorities, bool pickOverflows, [Nullable(new byte[]
		{
			2,
			1
		})] IReadOnlySet<string> disallowedJobs = null)
		{
			StationJobsSystem.<>c__DisplayClass25_0 CS$<>8__locals1 = new StationJobsSystem.<>c__DisplayClass25_0();
			CS$<>8__locals1.jobPriorities = jobPriorities;
			CS$<>8__locals1.disallowedJobs = disallowedJobs;
			CS$<>8__locals1.<>4__this = this;
			if (station == EntityUid.Invalid)
			{
				return null;
			}
			CS$<>8__locals1.available = this.GetAvailableJobs(station, null);
			string picked;
			if (CS$<>8__locals1.<PickBestAvailableJobWithPriority>g__TryPick|0(JobPriority.High, out picked))
			{
				return picked;
			}
			if (CS$<>8__locals1.<PickBestAvailableJobWithPriority>g__TryPick|0(JobPriority.Medium, out picked))
			{
				return picked;
			}
			if (CS$<>8__locals1.<PickBestAvailableJobWithPriority>g__TryPick|0(JobPriority.Low, out picked))
			{
				return picked;
			}
			if (!pickOverflows)
			{
				return null;
			}
			IReadOnlySet<string> overflows = this.GetOverflowJobs(station, null);
			if (overflows.Count == 0)
			{
				return null;
			}
			return RandomExtensions.Pick<string>(this._random, overflows);
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00028850 File Offset: 0x00026A50
		private TickerJobsAvailableEvent GenerateJobsAvailableEvent()
		{
			if (this._gameTicker.DisallowLateJoin)
			{
				return new TickerJobsAvailableEvent(new Dictionary<EntityUid, string>(), new Dictionary<EntityUid, Dictionary<string, uint?>>());
			}
			Dictionary<EntityUid, Dictionary<string, uint?>> jobs = new Dictionary<EntityUid, Dictionary<string, uint?>>();
			Dictionary<EntityUid, string> stationNames = new Dictionary<EntityUid, string>();
			foreach (EntityUid station in this._stationSystem.Stations)
			{
				Dictionary<string, uint?> list = base.Comp<StationJobsComponent>(station).JobList.ToDictionary((KeyValuePair<string, uint?> x) => x.Key, (KeyValuePair<string, uint?> x) => x.Value);
				jobs.Add(station, list);
				stationNames.Add(station, base.Name(station, null));
			}
			return new TickerJobsAvailableEvent(stationNames, jobs);
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00028934 File Offset: 0x00026B34
		private void UpdateJobsAvailable()
		{
			this._availableJobsDirty = true;
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0002893D File Offset: 0x00026B3D
		private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
		{
			base.RaiseNetworkEvent(this._cachedAvailableJobs, ev.PlayerSession.ConnectedClient);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x00028956 File Offset: 0x00026B56
		private void OnStationRenamed(EntityUid uid, StationJobsComponent component, StationRenamedEvent args)
		{
			this.UpdateJobsAvailable();
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x00028960 File Offset: 0x00026B60
		private void InitializeRoundStart()
		{
			this._jobsByWeight = new Dictionary<int, HashSet<string>>();
			foreach (JobPrototype job in this._prototypeManager.EnumeratePrototypes<JobPrototype>())
			{
				if (!this._jobsByWeight.ContainsKey(job.Weight))
				{
					this._jobsByWeight.Add(job.Weight, new HashSet<string>());
				}
				this._jobsByWeight[job.Weight].Add(job.ID);
			}
			this._orderedWeights = (from i in this._jobsByWeight.Keys
			orderby i descending
			select i).ToList<int>();
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x00028A38 File Offset: 0x00026C38
		[return: Nullable(new byte[]
		{
			1,
			0,
			2
		})]
		public Dictionary<NetUserId, ValueTuple<string, EntityUid>> AssignJobs(Dictionary<NetUserId, HumanoidCharacterProfile> profiles, IReadOnlyList<EntityUid> stations, bool useRoundStartJobs = true)
		{
			StationJobsSystem.<>c__DisplayClass38_0 CS$<>8__locals1;
			CS$<>8__locals1.profiles = profiles;
			this.InitializeRoundStart();
			if (CS$<>8__locals1.profiles.Count == 0)
			{
				return new Dictionary<NetUserId, ValueTuple<string, EntityUid>>();
			}
			CS$<>8__locals1.profiles = Extensions.ShallowClone<NetUserId, HumanoidCharacterProfile>(CS$<>8__locals1.profiles);
			CS$<>8__locals1.assigned = new Dictionary<NetUserId, ValueTuple<string, EntityUid>>(CS$<>8__locals1.profiles.Count);
			CS$<>8__locals1.stationJobs = new Dictionary<EntityUid, Dictionary<string, uint?>>();
			foreach (EntityUid station in stations)
			{
				if (useRoundStartJobs)
				{
					CS$<>8__locals1.stationJobs.Add(station, this.GetRoundStartJobs(station, null).ToDictionary((KeyValuePair<string, uint?> x) => x.Key, (KeyValuePair<string, uint?> x) => x.Value));
				}
				else
				{
					CS$<>8__locals1.stationJobs.Add(station, this.GetJobs(station, null).ToDictionary((KeyValuePair<string, uint?> x) => x.Key, (KeyValuePair<string, uint?> x) => x.Value));
				}
			}
			Dictionary<EntityUid, Dictionary<string, uint?>> currentlySelectingJobs = new Dictionary<EntityUid, Dictionary<string, uint?>>(stations.Count);
			foreach (EntityUid station2 in stations)
			{
				currentlySelectingJobs.Add(station2, new Dictionary<string, uint?>());
			}
			CS$<>8__locals1.jobPlayerOptions = new Dictionary<string, HashSet<NetUserId>>();
			Dictionary<EntityUid, int> stationTotalSlots = new Dictionary<EntityUid, int>(stations.Count);
			Dictionary<EntityUid, int> stationShares = new Dictionary<EntityUid, int>(stations.Count);
			foreach (int weight in this._orderedWeights)
			{
				for (JobPriority selectedPriority = JobPriority.High; selectedPriority > JobPriority.Never; selectedPriority--)
				{
					if (CS$<>8__locals1.profiles.Count == 0)
					{
						goto IL_6CE;
					}
					Dictionary<NetUserId, List<string>> candidates = this.GetPlayersJobCandidates(new int?(weight), new JobPriority?(selectedPriority), CS$<>8__locals1.profiles);
					StationJobsSystem.<>c__DisplayClass38_1 CS$<>8__locals2;
					CS$<>8__locals2.optionsRemaining = 0;
					CS$<>8__locals1.jobPlayerOptions.Clear();
					foreach (KeyValuePair<NetUserId, List<string>> keyValuePair in candidates)
					{
						NetUserId netUserId;
						List<string> list;
						keyValuePair.Deconstruct(out netUserId, out list);
						NetUserId user = netUserId;
						foreach (string job in list)
						{
							if (!CS$<>8__locals1.jobPlayerOptions.ContainsKey(job))
							{
								CS$<>8__locals1.jobPlayerOptions.Add(job, new HashSet<NetUserId>());
							}
							CS$<>8__locals1.jobPlayerOptions[job].Add(user);
						}
						int num = CS$<>8__locals2.optionsRemaining;
						CS$<>8__locals2.optionsRemaining = num + 1;
					}
					foreach (KeyValuePair<EntityUid, Dictionary<string, uint?>> slots in currentlySelectingJobs)
					{
						slots.Value.Clear();
					}
					foreach (EntityUid station3 in stations)
					{
						Dictionary<string, uint?> slots2 = currentlySelectingJobs[station3];
						foreach (KeyValuePair<string, uint?> keyValuePair2 in CS$<>8__locals1.stationJobs[station3])
						{
							string text;
							uint? num2;
							keyValuePair2.Deconstruct(out text, out num2);
							string job2 = text;
							uint? slot = num2;
							if (this._jobsByWeight[weight].Contains(job2))
							{
								slots2.Add(job2, slot);
							}
						}
					}
					stationTotalSlots.Clear();
					foreach (KeyValuePair<EntityUid, Dictionary<string, uint?>> keyValuePair3 in currentlySelectingJobs)
					{
						EntityUid entityUid;
						Dictionary<string, uint?> dictionary;
						keyValuePair3.Deconstruct(out entityUid, out dictionary);
						EntityUid station4 = entityUid;
						Dictionary<string, uint?> jobs = dictionary;
						stationTotalSlots.Add(station4, (int)jobs.Values.Sum((uint? x) => (long)((ulong)(x ?? 1U))));
					}
					int totalSlots = 0;
					foreach (KeyValuePair<EntityUid, int> keyValuePair4 in stationTotalSlots)
					{
						int num;
						EntityUid entityUid;
						keyValuePair4.Deconstruct(out entityUid, out num);
						int slot2 = num;
						totalSlots += slot2;
					}
					if (totalSlots != 0)
					{
						stationShares.Clear();
						int distributed = 0;
						foreach (EntityUid station5 in stations)
						{
							stationShares[station5] = (int)Math.Floor((double)((float)stationTotalSlots[station5] / (float)totalSlots * (float)candidates.Count));
							distributed += stationShares[station5];
						}
						if (distributed < candidates.Count)
						{
							EntityUid entityUid2 = RandomExtensions.Pick<EntityUid>(this._random, stations);
							Dictionary<EntityUid, int> dictionary2 = stationShares;
							EntityUid entityUid = entityUid2;
							dictionary2[entityUid] += candidates.Count - distributed;
						}
						foreach (EntityUid station6 in stations)
						{
							if (stationShares[station6] != 0)
							{
								Dictionary<string, uint?> currStationSelectingJobs = currentlySelectingJobs[station6];
								List<string> allJobs = currStationSelectingJobs.Keys.ToList<string>();
								this._random.Shuffle<string>(allJobs);
								int priorCount;
								do
								{
									priorCount = stationShares[station6];
									foreach (string job3 in allJobs)
									{
										if (stationShares[station6] == 0)
										{
											break;
										}
										uint? num2 = currStationSelectingJobs[job3];
										if (num2 != null)
										{
											num2 = currStationSelectingJobs[job3];
											uint num3 = 0U;
											if (num2.GetValueOrDefault() == num3 & num2 != null)
											{
												continue;
											}
										}
										if (CS$<>8__locals1.jobPlayerOptions.ContainsKey(job3))
										{
											StationJobsSystem.<AssignJobs>g__AssignPlayer|38_4(RandomExtensions.Pick<NetUserId>(this._random, CS$<>8__locals1.jobPlayerOptions[job3]), job3, station6, ref CS$<>8__locals1, ref CS$<>8__locals2);
											Dictionary<EntityUid, int> dictionary3 = stationShares;
											EntityUid entityUid = station6;
											int num = dictionary3[entityUid];
											dictionary3[entityUid] = num - 1;
											num2 = currStationSelectingJobs[job3];
											if (num2 != null)
											{
												Dictionary<string, uint?> dictionary4 = currStationSelectingJobs;
												string text = job3;
												num2 = dictionary4[text];
												dictionary4[text] = num2 - 1U;
											}
											if (CS$<>8__locals2.optionsRemaining == 0)
											{
												goto IL_6A4;
											}
										}
									}
								}
								while (priorCount != stationShares[station6]);
							}
						}
					}
					IL_6A4:;
				}
			}
			IL_6CE:
			return CS$<>8__locals1.assigned;
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x00029258 File Offset: 0x00027458
		public void AssignOverflowJobs([Nullable(new byte[]
		{
			1,
			0,
			2
		})] ref Dictionary<NetUserId, ValueTuple<string, EntityUid>> assignedJobs, IEnumerable<NetUserId> allPlayersToAssign, IReadOnlyDictionary<NetUserId, HumanoidCharacterProfile> profiles, IReadOnlyList<EntityUid> stations)
		{
			List<EntityUid> givenStations = stations.ToList<EntityUid>();
			if (givenStations.Count == 0)
			{
				return;
			}
			foreach (NetUserId player in allPlayersToAssign)
			{
				if (!assignedJobs.ContainsKey(player))
				{
					if (profiles[player].PreferenceUnavailable != PreferenceUnavailableMode.SpawnAsOverflow)
					{
						assignedJobs.Add(player, new ValueTuple<string, EntityUid>(null, EntityUid.Invalid));
					}
					else
					{
						this._random.Shuffle<EntityUid>(givenStations);
						foreach (EntityUid station in givenStations)
						{
							List<string> overflows = this.GetOverflowJobs(station, null).ToList<string>();
							this._random.Shuffle<string>(overflows);
							if (overflows.Count != 0)
							{
								assignedJobs.Add(player, new ValueTuple<string, EntityUid>(overflows[0], givenStations[0]));
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00029368 File Offset: 0x00027568
		public void CalcExtendedAccess(Dictionary<EntityUid, int> jobsCount)
		{
			foreach (KeyValuePair<EntityUid, int> keyValuePair in jobsCount)
			{
				EntityUid entityUid;
				int num;
				keyValuePair.Deconstruct(out entityUid, out num);
				EntityUid station = entityUid;
				int count = num;
				StationJobsComponent jobs = base.Comp<StationJobsComponent>(station);
				StationConfig stationConfig = base.Comp<StationDataComponent>(station).StationConfig;
				int thresh = (stationConfig != null) ? stationConfig.ExtendedAccessThreshold : -1;
				jobs.ExtendedAccess = (count <= thresh);
				Logger.DebugS("station", "Station {Station} on extended access: {ExtendedAccess}", new object[]
				{
					base.Name(station, null),
					jobs.ExtendedAccess
				});
			}
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00029424 File Offset: 0x00027624
		private Dictionary<NetUserId, List<string>> GetPlayersJobCandidates(int? weight, JobPriority? selectedPriority, Dictionary<NetUserId, HumanoidCharacterProfile> profiles)
		{
			Dictionary<NetUserId, List<string>> outputDict = new Dictionary<NetUserId, List<string>>(profiles.Count);
			foreach (KeyValuePair<NetUserId, HumanoidCharacterProfile> keyValuePair in profiles)
			{
				NetUserId netUserId;
				HumanoidCharacterProfile humanoidCharacterProfile;
				keyValuePair.Deconstruct(out netUserId, out humanoidCharacterProfile);
				NetUserId player = netUserId;
				HumanoidCharacterProfile profile = humanoidCharacterProfile;
				HashSet<string> roleBans = this._roleBanManager.GetJobBans(player);
				List<string> profileJobs = profile.JobPriorities.Keys.ToList<string>();
				this._playTime.RemoveDisallowedJobs(player, ref profileJobs);
				List<string> availableJobs = null;
				foreach (string jobId in profileJobs)
				{
					JobPriority jobPriority = profile.JobPriorities[jobId];
					JobPriority? jobPriority2 = selectedPriority;
					JobPrototype job;
					if (((jobPriority == jobPriority2.GetValueOrDefault() & jobPriority2 != null) || selectedPriority == null) && this._prototypeManager.TryIndex<JobPrototype>(jobId, ref job) && (weight == null || job.Weight == weight.Value) && (roleBans == null || !roleBans.Contains(jobId)))
					{
						if (availableJobs == null)
						{
							availableJobs = new List<string>(profile.JobPriorities.Count);
						}
						availableJobs.Add(jobId);
					}
				}
				if (availableJobs != null)
				{
					outputDict.Add(player, availableJobs);
				}
			}
			return outputDict;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x000295D4 File Offset: 0x000277D4
		[CompilerGenerated]
		internal static void <AssignJobs>g__AssignPlayer|38_4(NetUserId player, string job, EntityUid station, ref StationJobsSystem.<>c__DisplayClass38_0 A_3, ref StationJobsSystem.<>c__DisplayClass38_1 A_4)
		{
			string text;
			foreach (KeyValuePair<string, HashSet<NetUserId>> keyValuePair in A_3.jobPlayerOptions)
			{
				HashSet<NetUserId> hashSet;
				keyValuePair.Deconstruct(out text, out hashSet);
				string i = text;
				HashSet<NetUserId> hashSet2 = hashSet;
				hashSet2.Remove(player);
				if (hashSet2.Count == 0)
				{
					A_3.jobPlayerOptions.Remove(i);
				}
			}
			Dictionary<string, uint?> dictionary = A_3.stationJobs[station];
			text = job;
			uint? num = dictionary[text];
			dictionary[text] = num - 1U;
			A_3.profiles.Remove(player);
			A_3.assigned.Add(player, new ValueTuple<string, EntityUid>(job, station));
			int optionsRemaining = A_4.optionsRemaining;
			A_4.optionsRemaining = optionsRemaining - 1;
		}

		// Token: 0x040004E9 RID: 1257
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x040004EA RID: 1258
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040004EB RID: 1259
		[Dependency]
		private readonly GameTicker _gameTicker;

		// Token: 0x040004EC RID: 1260
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x040004ED RID: 1261
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040004EE RID: 1262
		private bool _availableJobsDirty;

		// Token: 0x040004EF RID: 1263
		private TickerJobsAvailableEvent _cachedAvailableJobs = new TickerJobsAvailableEvent(new Dictionary<EntityUid, string>(), new Dictionary<EntityUid, Dictionary<string, uint?>>());

		// Token: 0x040004F0 RID: 1264
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x040004F1 RID: 1265
		[Dependency]
		private readonly RoleBanManager _roleBanManager;

		// Token: 0x040004F2 RID: 1266
		[Dependency]
		private readonly PlayTimeTrackingSystem _playTime;

		// Token: 0x040004F3 RID: 1267
		private Dictionary<int, HashSet<string>> _jobsByWeight;

		// Token: 0x040004F4 RID: 1268
		private List<int> _orderedWeights;
	}
}
