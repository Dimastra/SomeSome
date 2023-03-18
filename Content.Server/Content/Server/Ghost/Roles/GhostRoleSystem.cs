using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.EUI;
using Content.Server.Ghost.Components;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Ghost.Roles.UI;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Follower;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Ghost.Roles;
using Content.Shared.Mobs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.Ghost.Roles
{
	// Token: 0x02000495 RID: 1173
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostRoleSystem : EntitySystem
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001786 RID: 6022 RVA: 0x0007B55C File Offset: 0x0007975C
		[ViewVariables]
		public IReadOnlyCollection<GhostRoleComponent> GhostRoles
		{
			get
			{
				return this._ghostRoles.Values;
			}
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0007B56C File Offset: 0x0007976C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.Reset), null, null);
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<GhostTakeoverAvailableComponent, MindAddedMessage>(new ComponentEventHandler<GhostTakeoverAvailableComponent, MindAddedMessage>(this.OnMindAdded), null, null);
			base.SubscribeLocalEvent<GhostTakeoverAvailableComponent, MindRemovedMessage>(new ComponentEventHandler<GhostTakeoverAvailableComponent, MindRemovedMessage>(this.OnMindRemoved), null, null);
			base.SubscribeLocalEvent<GhostTakeoverAvailableComponent, MobStateChangedEvent>(new ComponentEventHandler<GhostTakeoverAvailableComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
			base.SubscribeLocalEvent<GhostRoleComponent, ComponentInit>(new ComponentEventHandler<GhostRoleComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GhostRoleComponent, ComponentShutdown>(new ComponentEventHandler<GhostRoleComponent, ComponentShutdown>(this.OnShutdown), null, null);
			this._playerManager.PlayerStatusChanged += this.PlayerStatusChanged;
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0007B624 File Offset: 0x00079824
		private void OnMobStateChanged(EntityUid uid, GhostRoleComponent component, MobStateChangedEvent args)
		{
			MobState newMobState = args.NewMobState;
			if (newMobState != MobState.Alive)
			{
				if (newMobState - MobState.Critical > 1)
				{
					return;
				}
				this.UnregisterGhostRole(component);
			}
			else if (!component.Taken)
			{
				this.RegisterGhostRole(component);
				return;
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x0007B65B File Offset: 0x0007985B
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.PlayerStatusChanged;
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x0007B67C File Offset: 0x0007987C
		private uint GetNextRoleIdentifier()
		{
			uint nextRoleIdentifier = this._nextRoleIdentifier;
			this._nextRoleIdentifier = nextRoleIdentifier + 1U;
			return nextRoleIdentifier;
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x0007B69C File Offset: 0x0007989C
		public void OpenEui(IPlayerSession session)
		{
			EntityUid? attachedEntity = session.AttachedEntity;
			if (attachedEntity != null)
			{
				EntityUid attached = attachedEntity.GetValueOrDefault();
				if (attached.Valid && this.EntityManager.HasComponent<GhostComponent>(attached))
				{
					if (this._openUis.ContainsKey(session))
					{
						this.CloseEui(session);
					}
					GhostRolesEui eui = this._openUis[session] = new GhostRolesEui();
					this._euiManager.OpenEui(eui, session);
					eui.StateDirty();
					return;
				}
			}
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x0007B718 File Offset: 0x00079918
		public void OpenMakeGhostRoleEui(IPlayerSession session, EntityUid uid)
		{
			if (session.AttachedEntity == null)
			{
				return;
			}
			if (this._openMakeGhostRoleUis.ContainsKey(session))
			{
				this.CloseEui(session);
			}
			MakeGhostRoleEui eui = this._openMakeGhostRoleUis[session] = new MakeGhostRoleEui(uid);
			this._euiManager.OpenEui(eui, session);
			eui.StateDirty();
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x0007B774 File Offset: 0x00079974
		public void CloseEui(IPlayerSession session)
		{
			if (!this._openUis.ContainsKey(session))
			{
				return;
			}
			GhostRolesEui eui;
			this._openUis.Remove(session, out eui);
			if (eui != null)
			{
				eui.Close();
			}
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x0007B7A8 File Offset: 0x000799A8
		public void CloseMakeGhostRoleEui(IPlayerSession session)
		{
			MakeGhostRoleEui eui;
			if (this._openMakeGhostRoleUis.Remove(session, out eui))
			{
				eui.Close();
			}
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x0007B7CC File Offset: 0x000799CC
		public void UpdateAllEui()
		{
			foreach (GhostRolesEui ghostRolesEui in this._openUis.Values)
			{
				ghostRolesEui.StateDirty();
			}
			this._needsUpdateGhostRoleCount = true;
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x0007B828 File Offset: 0x00079A28
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (this._needsUpdateGhostRoleCount)
			{
				this._needsUpdateGhostRoleCount = false;
				GhostUpdateGhostRoleCountEvent response = new GhostUpdateGhostRoleCountEvent(this._ghostRoles.Count);
				foreach (ICommonSession player in this._playerManager.Sessions)
				{
					base.RaiseNetworkEvent(response, player.ConnectedClient);
				}
			}
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x0007B8A8 File Offset: 0x00079AA8
		private void PlayerStatusChanged([Nullable(2)] object blah, SessionStatusEventArgs args)
		{
			if (args.NewStatus == 3)
			{
				GhostUpdateGhostRoleCountEvent response = new GhostUpdateGhostRoleCountEvent(this._ghostRoles.Count);
				base.RaiseNetworkEvent(response, args.Session.ConnectedClient);
			}
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x0007B8E4 File Offset: 0x00079AE4
		public void RegisterGhostRole(GhostRoleComponent role)
		{
			if (this._ghostRoles.ContainsValue(role))
			{
				return;
			}
			this._ghostRoles[role.Identifier = this.GetNextRoleIdentifier()] = role;
			this.UpdateAllEui();
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x0007B924 File Offset: 0x00079B24
		public void UnregisterGhostRole(GhostRoleComponent role)
		{
			if (!this._ghostRoles.ContainsKey(role.Identifier) || this._ghostRoles[role.Identifier] != role)
			{
				return;
			}
			this._ghostRoles.Remove(role.Identifier);
			this.UpdateAllEui();
		}

		// Token: 0x06001794 RID: 6036 RVA: 0x0007B974 File Offset: 0x00079B74
		public void Takeover(IPlayerSession player, uint identifier)
		{
			GhostRoleComponent role;
			if (!this._ghostRoles.TryGetValue(identifier, out role))
			{
				return;
			}
			if (!role.Take(player))
			{
				return;
			}
			if (player.AttachedEntity != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.GhostRoleTaken;
				LogImpact impact = LogImpact.Low;
				LogStringHandler logStringHandler = new LogStringHandler(22, 3);
				logStringHandler.AppendFormatted<IPlayerSession>(player, "player", "player");
				logStringHandler.AppendLiteral(" took the ");
				logStringHandler.AppendFormatted(role.RoleName, 0, "roleName");
				logStringHandler.AppendLiteral(" ghost role ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player.AttachedEntity.Value), "entity", "ToPrettyString(player.AttachedEntity.Value)");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			this.CloseEui(player);
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x0007BA30 File Offset: 0x00079C30
		public void Follow(IPlayerSession player, uint identifier)
		{
			GhostRoleComponent role;
			if (!this._ghostRoles.TryGetValue(identifier, out role))
			{
				return;
			}
			if (player.AttachedEntity == null)
			{
				return;
			}
			this._followerSystem.StartFollowingEntity(player.AttachedEntity.Value, role.Owner);
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x0007BA80 File Offset: 0x00079C80
		public void GhostRoleInternalCreateMindAndTransfer(IPlayerSession player, EntityUid roleUid, EntityUid mob, [Nullable(2)] GhostRoleComponent role = null)
		{
			if (!base.Resolve<GhostRoleComponent>(roleUid, ref role, true))
			{
				return;
			}
			player.ContentData();
			Mind mind = new Mind(player.UserId);
			mind.CharacterName = this.EntityManager.GetComponent<MetaDataComponent>(mob).EntityName;
			mind.AddRole(new GhostRoleMarkerRole(mind, role.RoleName));
			mind.ChangeOwningPlayer(new NetUserId?(player.UserId));
			mind.TransferTo(new EntityUid?(mob), false, false);
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x0007BAF8 File Offset: 0x00079CF8
		public GhostRoleInfo[] GetGhostRolesInfo()
		{
			GhostRoleInfo[] roles = new GhostRoleInfo[this._ghostRoles.Count];
			int i = 0;
			foreach (KeyValuePair<uint, GhostRoleComponent> keyValuePair in this._ghostRoles)
			{
				uint num;
				GhostRoleComponent ghostRoleComponent;
				keyValuePair.Deconstruct(out num, out ghostRoleComponent);
				uint id = num;
				GhostRoleComponent role = ghostRoleComponent;
				roles[i] = new GhostRoleInfo
				{
					Identifier = id,
					Name = role.RoleName,
					Description = role.RoleDescription,
					Rules = role.RoleRules
				};
				i++;
			}
			return roles;
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x0007BBB4 File Offset: 0x00079DB4
		private void OnPlayerAttached(PlayerAttachedEvent message)
		{
			if (!this._openUis.ContainsKey(message.Player))
			{
				return;
			}
			if (this.EntityManager.HasComponent<GhostComponent>(message.Entity))
			{
				return;
			}
			this.CloseEui(message.Player);
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x0007BBEA File Offset: 0x00079DEA
		private void OnMindAdded(EntityUid uid, GhostTakeoverAvailableComponent component, MindAddedMessage args)
		{
			component.Taken = true;
			this.UnregisterGhostRole(component);
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x0007BBFA File Offset: 0x00079DFA
		private void OnMindRemoved(EntityUid uid, GhostRoleComponent component, MindRemovedMessage args)
		{
			if (!component.ReregisterOnGhost || component.LifeStage > 6)
			{
				return;
			}
			component.Taken = false;
			this.RegisterGhostRole(component);
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x0007BC1C File Offset: 0x00079E1C
		public void Reset(RoundRestartCleanupEvent ev)
		{
			foreach (IPlayerSession session in this._openUis.Keys)
			{
				this.CloseEui(session);
			}
			this._openUis.Clear();
			this._ghostRoles.Clear();
			this._nextRoleIdentifier = 0U;
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0007BC94 File Offset: 0x00079E94
		private void OnInit(EntityUid uid, GhostRoleComponent role, ComponentInit args)
		{
			if (role.Probability < 1f && !RandomExtensions.Prob(this._random, role.Probability))
			{
				base.RemComp<GhostRoleComponent>(uid);
				return;
			}
			if (role.RoleRules == "")
			{
				role.RoleRules = Loc.GetString("ghost-role-component-default-rules");
			}
			this.RegisterGhostRole(role);
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x0007BCF3 File Offset: 0x00079EF3
		private void OnShutdown(EntityUid uid, GhostRoleComponent role, ComponentShutdown args)
		{
			this.UnregisterGhostRole(role);
		}

		// Token: 0x04000EA2 RID: 3746
		[Dependency]
		private readonly EuiManager _euiManager;

		// Token: 0x04000EA3 RID: 3747
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000EA4 RID: 3748
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04000EA5 RID: 3749
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000EA6 RID: 3750
		[Dependency]
		private readonly FollowerSystem _followerSystem;

		// Token: 0x04000EA7 RID: 3751
		private uint _nextRoleIdentifier;

		// Token: 0x04000EA8 RID: 3752
		private bool _needsUpdateGhostRoleCount = true;

		// Token: 0x04000EA9 RID: 3753
		private readonly Dictionary<uint, GhostRoleComponent> _ghostRoles = new Dictionary<uint, GhostRoleComponent>();

		// Token: 0x04000EAA RID: 3754
		private readonly Dictionary<IPlayerSession, GhostRolesEui> _openUis = new Dictionary<IPlayerSession, GhostRolesEui>();

		// Token: 0x04000EAB RID: 3755
		private readonly Dictionary<IPlayerSession, MakeGhostRoleEui> _openMakeGhostRoleUis = new Dictionary<IPlayerSession, MakeGhostRoleEui>();
	}
}
