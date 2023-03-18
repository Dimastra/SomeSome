using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.GameTicking;
using Content.Server.Ghost.Components;
using Content.Server.Mind.Components;
using Content.Server.Objectives;
using Content.Server.Objectives.Interfaces;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mind
{
	// Token: 0x020003A0 RID: 928
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class Mind
	{
		// Token: 0x060012EA RID: 4842 RVA: 0x00061E7C File Offset: 0x0006007C
		public Mind(NetUserId userId)
		{
			this.OriginalOwnerUserId = userId;
			IoCManager.InjectDependencies<Mind>(this);
			this._entityManager.EntitySysManager.Resolve<MobStateSystem>(ref this._mobStateSystem);
			this._entityManager.EntitySysManager.Resolve<GameTicker>(ref this._gameTickerSystem);
			this._entityManager.EntitySysManager.Resolve<MindSystem>(ref this._mindSystem);
		}

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x00061F00 File Offset: 0x00060100
		// (set) Token: 0x060012EC RID: 4844 RVA: 0x00061F08 File Offset: 0x00060108
		[ViewVariables]
		public NetUserId? UserId { get; private set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060012ED RID: 4845 RVA: 0x00061F11 File Offset: 0x00060111
		[ViewVariables]
		public NetUserId OriginalOwnerUserId { get; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060012EE RID: 4846 RVA: 0x00061F1C File Offset: 0x0006011C
		[ViewVariables]
		public bool IsVisitingEntity
		{
			get
			{
				return this.VisitingEntity != null;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060012EF RID: 4847 RVA: 0x00061F37 File Offset: 0x00060137
		// (set) Token: 0x060012F0 RID: 4848 RVA: 0x00061F3F File Offset: 0x0006013F
		[ViewVariables]
		public EntityUid? VisitingEntity { get; private set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060012F1 RID: 4849 RVA: 0x00061F48 File Offset: 0x00060148
		[ViewVariables]
		public EntityUid? CurrentEntity
		{
			get
			{
				EntityUid? visitingEntity = this.VisitingEntity;
				if (visitingEntity == null)
				{
					return this.OwnedEntity;
				}
				return visitingEntity;
			}
		}

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060012F2 RID: 4850 RVA: 0x00061F6D File Offset: 0x0006016D
		// (set) Token: 0x060012F3 RID: 4851 RVA: 0x00061F75 File Offset: 0x00060175
		[Nullable(2)]
		[ViewVariables]
		public string CharacterName { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060012F4 RID: 4852 RVA: 0x00061F7E File Offset: 0x0006017E
		// (set) Token: 0x060012F5 RID: 4853 RVA: 0x00061F86 File Offset: 0x00060186
		[ViewVariables]
		public TimeSpan? TimeOfDeath { get; set; }

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060012F6 RID: 4854 RVA: 0x00061F8F File Offset: 0x0006018F
		// (set) Token: 0x060012F7 RID: 4855 RVA: 0x00061F97 File Offset: 0x00060197
		[Nullable(2)]
		[ViewVariables]
		public MindComponent OwnedComponent { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x00061FA0 File Offset: 0x000601A0
		[ViewVariables]
		public EntityUid? OwnedEntity
		{
			get
			{
				MindComponent ownedComponent = this.OwnedComponent;
				if (ownedComponent == null)
				{
					return null;
				}
				return new EntityUid?(ownedComponent.Owner);
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x00061FCB File Offset: 0x000601CB
		[ViewVariables]
		public IEnumerable<Role> AllRoles
		{
			get
			{
				return this._roles;
			}
		}

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060012FA RID: 4858 RVA: 0x00061FD3 File Offset: 0x000601D3
		[ViewVariables]
		public IEnumerable<Objective> AllObjectives
		{
			get
			{
				return this._objectives;
			}
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x00061FDB File Offset: 0x000601DB
		// (set) Token: 0x060012FC RID: 4860 RVA: 0x00061FE3 File Offset: 0x000601E3
		[ViewVariables]
		[DataField("preventGhosting", false, 1, false, false, null)]
		public bool PreventGhosting { get; set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060012FD RID: 4861 RVA: 0x00061FEC File Offset: 0x000601EC
		// (set) Token: 0x060012FE RID: 4862 RVA: 0x00061FF4 File Offset: 0x000601F4
		[ViewVariables]
		[DataField("preventSuicide", false, 1, false, false, null)]
		public bool PreventSuicide { get; set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060012FF RID: 4863 RVA: 0x00062000 File Offset: 0x00060200
		[Nullable(2)]
		[ViewVariables]
		public IPlayerSession Session
		{
			[NullableContext(2)]
			get
			{
				if (this.UserId == null)
				{
					return null;
				}
				IPlayerSession ret;
				this._playerManager.TryGetSessionById(this.UserId.Value, ref ret);
				return ret;
			}
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001300 RID: 4864 RVA: 0x0006203C File Offset: 0x0006023C
		[ViewVariables]
		public bool CharacterDeadIC
		{
			get
			{
				return this.CharacterDeadPhysically;
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001301 RID: 4865 RVA: 0x00062044 File Offset: 0x00060244
		[ViewVariables]
		public bool CharacterDeadPhysically
		{
			get
			{
				MobStateComponent targetMobState = EntityManagerExt.GetComponentOrNull<MobStateComponent>(this._entityManager, this.OwnedEntity);
				return targetMobState == null || this._mobStateSystem.IsDead(this.OwnedEntity.Value, targetMobState);
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06001302 RID: 4866 RVA: 0x00062084 File Offset: 0x00060284
		private string MindOwnerLoggingString
		{
			get
			{
				if (this.OwnedEntity != null)
				{
					return this._entityManager.ToPrettyString(this.OwnedEntity.Value);
				}
				if (this.UserId != null)
				{
					return this.UserId.Value.ToString();
				}
				return "(originally " + this.OriginalOwnerUserId.ToString() + ")";
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x00062110 File Offset: 0x00060310
		public Role AddRole(Role role)
		{
			if (this._roles.Contains(role))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("We already have this role: ");
				defaultInterpolatedStringHandler.AppendFormatted<Role>(role);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this._roles.Add(role);
			role.Greet();
			RoleAddedEvent message = new RoleAddedEvent(this, role);
			if (this.OwnedEntity != null)
			{
				this._entityManager.EventBus.RaiseLocalEvent<RoleAddedEvent>(this.OwnedEntity.Value, message, true);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Mind;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(20, 2);
			logStringHandler.AppendLiteral("'");
			logStringHandler.AppendFormatted(role.Name);
			logStringHandler.AppendLiteral("' added to mind of ");
			logStringHandler.AppendFormatted(this.MindOwnerLoggingString);
			adminLogger.Add(type, impact, ref logStringHandler);
			return role;
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x000621F0 File Offset: 0x000603F0
		public void RemoveRole(Role role)
		{
			if (!this._roles.Contains(role))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("We do not have this role: ");
				defaultInterpolatedStringHandler.AppendFormatted<Role>(role);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this._roles.Remove(role);
			RoleRemovedEvent message = new RoleRemovedEvent(this, role);
			if (this.OwnedEntity != null)
			{
				this._entityManager.EventBus.RaiseLocalEvent<RoleRemovedEvent>(this.OwnedEntity.Value, message, true);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Mind;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(24, 2);
			logStringHandler.AppendLiteral("'");
			logStringHandler.AppendFormatted(role.Name);
			logStringHandler.AppendLiteral("' removed from mind of ");
			logStringHandler.AppendFormatted(this.MindOwnerLoggingString);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000622C8 File Offset: 0x000604C8
		[NullableContext(0)]
		public bool HasRole<T>() where T : Role
		{
			Type t = typeof(T);
			return this._roles.Any((Role role) => role.GetType() == t);
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06001306 RID: 4870 RVA: 0x00062302 File Offset: 0x00060502
		[Nullable(2)]
		public Job CurrentJob
		{
			[NullableContext(2)]
			get
			{
				return this._roles.OfType<Job>().SingleOrDefault<Job>();
			}
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x00062314 File Offset: 0x00060514
		public bool TryAddObjective(ObjectivePrototype objectivePrototype)
		{
			if (!objectivePrototype.CanBeAssigned(this))
			{
				return false;
			}
			Objective objective = objectivePrototype.GetObjective(this);
			if (this._objectives.Contains(objective))
			{
				return false;
			}
			foreach (IObjectiveCondition condition in objective.Conditions)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Mind;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(20, 2);
				logStringHandler.AppendLiteral("'");
				logStringHandler.AppendFormatted(condition.Title);
				logStringHandler.AppendLiteral("' added to mind of ");
				logStringHandler.AppendFormatted(this.MindOwnerLoggingString);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._objectives.Add(objective);
			return true;
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000623D8 File Offset: 0x000605D8
		public bool TryRemoveObjective(int index)
		{
			if (this._objectives.Count >= index)
			{
				return false;
			}
			Objective objective = this._objectives[index];
			foreach (IObjectiveCondition condition in objective.Conditions)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Mind;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(28, 2);
				logStringHandler.AppendLiteral("'");
				logStringHandler.AppendFormatted(condition.Title);
				logStringHandler.AppendLiteral("' removed from the mind of ");
				logStringHandler.AppendFormatted(this.MindOwnerLoggingString);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this._objectives.Remove(objective);
			return true;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x00062494 File Offset: 0x00060694
		public void TransferTo(EntityUid? entity, bool ghostCheckOverride = false, bool preserveProperties = false)
		{
			if (entity == this.OwnedEntity)
			{
				this.UnVisit();
				return;
			}
			MindComponent component = null;
			bool alreadyAttached = false;
			if (entity != null)
			{
				if (!this._entityManager.TryGetComponent<MindComponent>(entity.Value, ref component))
				{
					component = this._entityManager.AddComponent<MindComponent>(entity.Value);
				}
				else if (component.HasMind)
				{
					this._gameTickerSystem.OnGhostAttempt(component.Mind, false, false);
				}
				ActorComponent actor;
				if (this._entityManager.TryGetComponent<ActorComponent>(entity.Value, ref actor))
				{
					if (actor.PlayerSession != this.Session)
					{
						throw new ArgumentException("Visit target already has a session.", "entity");
					}
					alreadyAttached = true;
				}
			}
			if (this.OwnedComponent != null)
			{
				this._mindSystem.InternalEjectMind(this.OwnedComponent.Owner, this.OwnedComponent);
			}
			this.OwnedComponent = component;
			if (this.OwnedComponent != null)
			{
				this._mindSystem.InternalAssignMind(this.OwnedComponent.Owner, this, this.OwnedComponent);
			}
			GhostComponent ghostComponent;
			if (alreadyAttached)
			{
				this.VisitingEntity = null;
				this._entityManager.RemoveComponent<VisitingMindComponent>(entity.Value);
			}
			else if (this.VisitingEntity != null && (ghostCheckOverride || !this._entityManager.TryGetComponent<GhostComponent>(this.VisitingEntity, ref ghostComponent) || !ghostComponent.CanReturnToBody))
			{
				this.RemoveVisitingEntity();
			}
			if (this.Session != null && !alreadyAttached && this.VisitingEntity == null)
			{
				this.Session.AttachToEntity(entity);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Session ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Session.Name);
				defaultInterpolatedStringHandler.AppendLiteral(" transferred to entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(entity);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0006269C File Offset: 0x0006089C
		public void ChangeOwningPlayer(NetUserId? newOwner)
		{
			IPlayerManager playerMgr = IoCManager.Resolve<IPlayerManager>();
			PlayerData newOwnerData = null;
			if (newOwner != null)
			{
				IPlayerData uncast;
				if (!playerMgr.TryGetPlayerData(newOwner.Value, ref uncast))
				{
					throw new ArgumentException("new owner must have previously logged into the server.");
				}
				newOwnerData = uncast.ContentData();
			}
			IPlayerSession session = this.Session;
			if (session != null)
			{
				session.AttachToEntity(null);
			}
			if (this.UserId != null)
			{
				playerMgr.GetPlayerData(this.UserId.Value).ContentData().UpdateMindFromMindChangeOwningPlayer(null);
			}
			this.UserId = newOwner;
			if (newOwner == null)
			{
				return;
			}
			Mind mind = newOwnerData.Mind;
			if (mind != null)
			{
				mind.ChangeOwningPlayer(null);
			}
			newOwnerData.UpdateMindFromMindChangeOwningPlayer(this);
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0006275C File Offset: 0x0006095C
		public void Visit(EntityUid entity)
		{
			IPlayerSession session = this.Session;
			if (session != null)
			{
				session.AttachToEntity(entity);
			}
			this.VisitingEntity = new EntityUid?(entity);
			this._entityManager.AddComponent<VisitingMindComponent>(entity).Mind = this;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Session ");
			IPlayerSession session2 = this.Session;
			defaultInterpolatedStringHandler.AppendFormatted((session2 != null) ? session2.Name : null);
			defaultInterpolatedStringHandler.AppendLiteral(" visiting entity ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entity);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x000627F4 File Offset: 0x000609F4
		public void UnVisit()
		{
			IPlayerSession session = this.Session;
			EntityUid? currentEntity = (session != null) ? session.AttachedEntity : null;
			IPlayerSession session2 = this.Session;
			if (session2 != null)
			{
				session2.AttachToEntity(this.OwnedEntity);
			}
			this.RemoveVisitingEntity();
			if (this.Session != null && this.OwnedEntity != null && currentEntity != this.OwnedEntity)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Mind;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(13, 2);
				logStringHandler.AppendFormatted(this.Session.Name);
				logStringHandler.AppendLiteral(" returned to ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entityManager.ToPrettyString(this.OwnedEntity.Value), "_entityManager.ToPrettyString(OwnedEntity.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x000628F4 File Offset: 0x00060AF4
		private void RemoveVisitingEntity()
		{
			if (this.VisitingEntity == null)
			{
				return;
			}
			EntityUid oldVisitingEnt = this.VisitingEntity.Value;
			this.VisitingEntity = null;
			this._entityManager.RemoveComponent<VisitingMindComponent>(oldVisitingEnt);
			this._entityManager.EventBus.RaiseLocalEvent<MindUnvisitedMessage>(oldVisitingEnt, new MindUnvisitedMessage(), true);
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x00062954 File Offset: 0x00060B54
		[NullableContext(2)]
		public bool TryGetSession([NotNullWhen(true)] out IPlayerSession session)
		{
			IPlayerSession session2;
			session = (session2 = this.Session);
			return session2 != null;
		}

		// Token: 0x04000B96 RID: 2966
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000B97 RID: 2967
		private readonly GameTicker _gameTickerSystem;

		// Token: 0x04000B98 RID: 2968
		private readonly MindSystem _mindSystem;

		// Token: 0x04000B99 RID: 2969
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000B9A RID: 2970
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000B9B RID: 2971
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000B9C RID: 2972
		private readonly ISet<Role> _roles = new HashSet<Role>();

		// Token: 0x04000B9D RID: 2973
		private readonly List<Objective> _objectives = new List<Objective>();

		// Token: 0x04000B9E RID: 2974
		public string Briefing = string.Empty;
	}
}
