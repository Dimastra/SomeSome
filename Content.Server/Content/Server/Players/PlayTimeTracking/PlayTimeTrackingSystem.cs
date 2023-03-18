using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Afk;
using Content.Server.Afk.Events;
using Content.Server.GameTicking;
using Content.Server.Mind;
using Content.Server.Roles;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server.Players.PlayTimeTracking
{
	// Token: 0x020002D5 RID: 725
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayTimeTrackingSystem : EntitySystem
	{
		// Token: 0x06000EAC RID: 3756 RVA: 0x0004A64C File Offset: 0x0004884C
		public override void Initialize()
		{
			base.Initialize();
			this._tracking.CalcTrackers += this.CalcTrackers;
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundEnd), null, null);
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<RoleAddedEvent>(new EntityEventHandler<RoleAddedEvent>(this.OnRoleAdd), null, null);
			base.SubscribeLocalEvent<RoleRemovedEvent>(new EntityEventHandler<RoleRemovedEvent>(this.OnRoleRemove), null, null);
			base.SubscribeLocalEvent<AFKEvent>(new EntityEventRefHandler<AFKEvent>(this.OnAFK), null, null);
			base.SubscribeLocalEvent<UnAFKEvent>(new EntityEventRefHandler<UnAFKEvent>(this.OnUnAFK), null, null);
			base.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<PlayerJoinedLobbyEvent>(new EntityEventHandler<PlayerJoinedLobbyEvent>(this.OnPlayerJoinedLobby), null, null);
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0004A72A File Offset: 0x0004892A
		public override void Shutdown()
		{
			base.Shutdown();
			this._tracking.CalcTrackers -= this.CalcTrackers;
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0004A749 File Offset: 0x00048949
		private void CalcTrackers(IPlayerSession player, HashSet<string> trackers)
		{
			if (this._afk.IsAfk(player))
			{
				return;
			}
			if (!this.IsPlayerAlive(player))
			{
				return;
			}
			trackers.Add("Overall");
			trackers.UnionWith(this.GetTimedRoles(player));
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0004A780 File Offset: 0x00048980
		private bool IsPlayerAlive(IPlayerSession session)
		{
			EntityUid? attached = session.AttachedEntity;
			if (attached == null)
			{
				return false;
			}
			MobStateComponent state;
			if (!base.TryComp<MobStateComponent>(attached, ref state))
			{
				return false;
			}
			MobState currentState = state.CurrentState;
			return currentState == MobState.Alive || currentState == MobState.Critical;
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0004A7C4 File Offset: 0x000489C4
		public IEnumerable<string> GetTimedRoles(Mind mind)
		{
			foreach (Role role in mind.AllRoles)
			{
				IRoleTimer timer = role as IRoleTimer;
				if (timer != null)
				{
					yield return this._prototypes.Index<PlayTimeTrackerPrototype>(timer.Timer).ID;
				}
			}
			IEnumerator<Role> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0004A7DC File Offset: 0x000489DC
		private IEnumerable<string> GetTimedRoles(IPlayerSession session)
		{
			PlayerData contentData = this._playerManager.GetPlayerData(session.UserId).ContentData();
			if (((contentData != null) ? contentData.Mind : null) == null)
			{
				return Enumerable.Empty<string>();
			}
			return this.GetTimedRoles(contentData.Mind);
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0004A820 File Offset: 0x00048A20
		private void OnRoleRemove(RoleRemovedEvent ev)
		{
			if (ev.Mind.Session == null)
			{
				return;
			}
			this._tracking.QueueRefreshTrackers(ev.Mind.Session);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0004A846 File Offset: 0x00048A46
		private void OnRoleAdd(RoleAddedEvent ev)
		{
			if (ev.Mind.Session == null)
			{
				return;
			}
			this._tracking.QueueRefreshTrackers(ev.Mind.Session);
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x0004A86C File Offset: 0x00048A6C
		private void OnRoundEnd(RoundRestartCleanupEvent ev)
		{
			this._tracking.Save();
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x0004A879 File Offset: 0x00048A79
		private void OnUnAFK(ref UnAFKEvent ev)
		{
			this._tracking.QueueRefreshTrackers(ev.Session);
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x0004A88C File Offset: 0x00048A8C
		private void OnAFK(ref AFKEvent ev)
		{
			this._tracking.QueueRefreshTrackers(ev.Session);
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0004A89F File Offset: 0x00048A9F
		private void OnPlayerAttached(PlayerAttachedEvent ev)
		{
			this._tracking.QueueRefreshTrackers(ev.Player);
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0004A8B2 File Offset: 0x00048AB2
		private void OnPlayerDetached(PlayerDetachedEvent ev)
		{
			this._tracking.QueueRefreshTrackers(ev.Player);
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x0004A8C8 File Offset: 0x00048AC8
		private void OnMobStateChanged(MobStateChangedEvent ev)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(ev.Target, ref actor))
			{
				return;
			}
			this._tracking.QueueRefreshTrackers(actor.PlayerSession);
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0004A8F8 File Offset: 0x00048AF8
		private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
		{
			this._tracking.QueueRefreshTrackers(ev.PlayerSession);
			this._tracking.QueueSendTimers(ev.PlayerSession);
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x0004A91C File Offset: 0x00048B1C
		public bool IsAllowed(IPlayerSession player, string role)
		{
			JobPrototype job;
			if (!this._prototypes.TryIndex<JobPrototype>(role, ref job) || job.Requirements == null || !this._cfg.GetCVar<bool>(CCVars.GameRoleTimers))
			{
				return true;
			}
			Dictionary<string, TimeSpan> playTimes = this._tracking.GetTrackerTimes(player);
			string text;
			return JobRequirements.TryRequirementsMet(job, playTimes, out text, this._prototypes);
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x0004A974 File Offset: 0x00048B74
		public HashSet<string> GetDisallowedJobs(IPlayerSession player)
		{
			HashSet<string> roles = new HashSet<string>();
			if (!this._cfg.GetCVar<bool>(CCVars.GameRoleTimers))
			{
				return roles;
			}
			Dictionary<string, TimeSpan> playTimes = this._tracking.GetTrackerTimes(player);
			using (IEnumerator<JobPrototype> enumerator = this._prototypes.EnumeratePrototypes<JobPrototype>().GetEnumerator())
			{
				IL_97:
				while (enumerator.MoveNext())
				{
					JobPrototype job = enumerator.Current;
					if (job.Requirements != null)
					{
						using (HashSet<JobRequirement>.Enumerator enumerator2 = job.Requirements.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string text;
								if (!JobRequirements.TryRequirementMet(enumerator2.Current, playTimes, out text, this._prototypes))
								{
									goto IL_97;
								}
							}
						}
					}
					roles.Add(job.ID);
				}
			}
			return roles;
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x0004AA4C File Offset: 0x00048C4C
		public void RemoveDisallowedJobs(NetUserId userId, ref List<string> jobs)
		{
			if (!this._cfg.GetCVar<bool>(CCVars.GameRoleTimers))
			{
				return;
			}
			IPlayerSession player = this._playerManager.GetSessionByUserId(userId);
			Dictionary<string, TimeSpan> playTimes = this._tracking.GetTrackerTimes(player);
			for (int i = 0; i < jobs.Count; i++)
			{
				string job = jobs[i];
				JobPrototype jobber;
				if (this._prototypes.TryIndex<JobPrototype>(job, ref jobber) && jobber.Requirements != null && jobber.Requirements.Count != 0)
				{
					using (HashSet<JobRequirement>.Enumerator enumerator = jobber.Requirements.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text;
							if (!JobRequirements.TryRequirementMet(enumerator.Current, playTimes, out text, this._prototypes))
							{
								Extensions.RemoveSwap<string>(jobs, i);
								i--;
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0004AB30 File Offset: 0x00048D30
		public void PlayerRolesChanged(IPlayerSession player)
		{
			this._tracking.QueueRefreshTrackers(player);
		}

		// Token: 0x040008A6 RID: 2214
		[Dependency]
		private readonly IAfkManager _afk;

		// Token: 0x040008A7 RID: 2215
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040008A8 RID: 2216
		[Dependency]
		private readonly IPrototypeManager _prototypes;

		// Token: 0x040008A9 RID: 2217
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040008AA RID: 2218
		[Dependency]
		private readonly PlayTimeTrackingManager _tracking;
	}
}
