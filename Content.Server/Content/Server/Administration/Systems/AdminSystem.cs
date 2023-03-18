using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.IdentityManagement;
using Content.Server.Mind;
using Content.Server.Players;
using Content.Server.Roles;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.GameTicking;
using Content.Shared.IdentityManagement;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Administration.Systems
{
	// Token: 0x0200080B RID: 2059
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminSystem : EntitySystem
	{
		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06002CAC RID: 11436 RVA: 0x000E8AEC File Offset: 0x000E6CEC
		public IReadOnlySet<NetUserId> RoundActivePlayers
		{
			get
			{
				return this._roundActivePlayers;
			}
		}

		// Token: 0x06002CAD RID: 11437 RVA: 0x000E8AF4 File Offset: 0x000E6CF4
		public override void Initialize()
		{
			base.Initialize();
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			this._adminManager.OnPermsChanged += this.OnAdminPermsChanged;
			base.SubscribeLocalEvent<IdentityChangedEvent>(new EntityEventHandler<IdentityChangedEvent>(this.OnIdentityChanged), null, null);
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			base.SubscribeLocalEvent<RoleAddedEvent>(new EntityEventHandler<RoleAddedEvent>(this.OnRoleEvent), null, null);
			base.SubscribeLocalEvent<RoleRemovedEvent>(new EntityEventHandler<RoleRemovedEvent>(this.OnRoleEvent), null, null);
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup), null, null);
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x000E8BB0 File Offset: 0x000E6DB0
		private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
		{
			this._roundActivePlayers.Clear();
			foreach (KeyValuePair<NetUserId, PlayerInfo> keyValuePair in this._playerList)
			{
				NetUserId netUserId;
				PlayerInfo playerInfo;
				keyValuePair.Deconstruct(out netUserId, out playerInfo);
				NetUserId id = netUserId;
				if (playerInfo.ActiveThisRound)
				{
					IPlayerData playerData;
					if (!this._playerManager.TryGetPlayerData(id, ref playerData))
					{
						return;
					}
					IPlayerSession session;
					this._playerManager.TryGetSessionById(id, ref session);
					this._playerList[id] = this.GetPlayerInfo(playerData, session);
				}
			}
			FullPlayerListEvent updateEv = new FullPlayerListEvent
			{
				PlayersInfo = this._playerList.Values.ToList<PlayerInfo>()
			};
			foreach (IPlayerSession admin in this._adminManager.ActiveAdmins)
			{
				base.RaiseNetworkEvent(updateEv, admin.ConnectedClient);
			}
		}

		// Token: 0x06002CAF RID: 11439 RVA: 0x000E8CC4 File Offset: 0x000E6EC4
		public void UpdatePlayerList(IPlayerSession player)
		{
			this._playerList[player.UserId] = this.GetPlayerInfo(player.Data, player);
			PlayerInfoChangedEvent playerInfoChangedEvent = new PlayerInfoChangedEvent
			{
				PlayerInfo = this._playerList[player.UserId]
			};
			foreach (IPlayerSession admin in this._adminManager.ActiveAdmins)
			{
				base.RaiseNetworkEvent(playerInfoChangedEvent, admin.ConnectedClient);
			}
		}

		// Token: 0x06002CB0 RID: 11440 RVA: 0x000E8D58 File Offset: 0x000E6F58
		private void OnIdentityChanged(IdentityChangedEvent ev)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(ev.CharacterEntity, ref actor))
			{
				return;
			}
			this.UpdatePlayerList(actor.PlayerSession);
		}

		// Token: 0x06002CB1 RID: 11441 RVA: 0x000E8D82 File Offset: 0x000E6F82
		private void OnRoleEvent(RoleEvent ev)
		{
			if (!ev.Role.Antagonist || ev.Role.Mind.Session == null)
			{
				return;
			}
			this.UpdatePlayerList(ev.Role.Mind.Session);
		}

		// Token: 0x06002CB2 RID: 11442 RVA: 0x000E8DBA File Offset: 0x000E6FBA
		private void OnAdminPermsChanged(AdminPermsChangedEventArgs obj)
		{
			if (!obj.IsAdmin)
			{
				base.RaiseNetworkEvent(new FullPlayerListEvent(), obj.Player.ConnectedClient);
				return;
			}
			this.SendFullPlayerList(obj.Player);
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x000E8DE7 File Offset: 0x000E6FE7
		private void OnPlayerDetached(PlayerDetachedEvent ev)
		{
			if (ev.Player.Status == 4)
			{
				return;
			}
			this.UpdatePlayerList(ev.Player);
		}

		// Token: 0x06002CB4 RID: 11444 RVA: 0x000E8E04 File Offset: 0x000E7004
		private void OnPlayerAttached(PlayerAttachedEvent ev)
		{
			if (ev.Player.Status == 4)
			{
				return;
			}
			this._roundActivePlayers.Add(ev.Player.UserId);
			this.UpdatePlayerList(ev.Player);
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x000E8E38 File Offset: 0x000E7038
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
			this._adminManager.OnPermsChanged -= this.OnAdminPermsChanged;
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x000E8E6E File Offset: 0x000E706E
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			this.UpdatePlayerList(e.Session);
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x000E8E7C File Offset: 0x000E707C
		private void SendFullPlayerList(IPlayerSession playerSession)
		{
			base.RaiseNetworkEvent(new FullPlayerListEvent
			{
				PlayersInfo = this._playerList.Values.ToList<PlayerInfo>()
			}, playerSession.ConnectedClient);
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x000E8EB4 File Offset: 0x000E70B4
		private PlayerInfo GetPlayerInfo(IPlayerData data, [Nullable(2)] IPlayerSession session)
		{
			string userName = data.UserName;
			string entityName = string.Empty;
			string identityName = string.Empty;
			if (session != null && session.AttachedEntity != null)
			{
				entityName = this.EntityManager.GetComponent<MetaDataComponent>(session.AttachedEntity.Value).EntityName;
				identityName = Identity.Name(session.AttachedEntity.Value, this.EntityManager, null);
			}
			PlayerData playerData = data.ContentData();
			Mind mind = (playerData != null) ? playerData.Mind : null;
			Role role2;
			if (mind == null)
			{
				role2 = null;
			}
			else
			{
				role2 = mind.AllRoles.FirstOrDefault((Role role) => role is Job);
			}
			Role job = role2;
			string startingRole = (job != null) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(job.Name) : string.Empty;
			bool flag;
			if (mind == null)
			{
				flag = false;
			}
			else
			{
				flag = mind.AllRoles.Any((Role r) => r.Antagonist);
			}
			bool antag = flag;
			bool flag2;
			if (session != null)
			{
				SessionStatus status = session.Status;
				flag2 = (status == 2 || status == 3);
			}
			else
			{
				flag2 = false;
			}
			bool connected = flag2;
			return new PlayerInfo(userName, entityName, identityName, startingRole, antag, (session != null) ? session.AttachedEntity : null, data.UserId, connected, this._roundActivePlayers.Contains(data.UserId));
		}

		// Token: 0x04001B91 RID: 7057
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B92 RID: 7058
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B93 RID: 7059
		private readonly Dictionary<NetUserId, PlayerInfo> _playerList = new Dictionary<NetUserId, PlayerInfo>();

		// Token: 0x04001B94 RID: 7060
		private readonly HashSet<NetUserId> _roundActivePlayers = new HashSet<NetUserId>();
	}
}
