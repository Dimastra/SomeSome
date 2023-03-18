using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Administration.Systems
{
	// Token: 0x020004DC RID: 1244
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminSystem : EntitySystem
	{
		// Token: 0x140000BF RID: 191
		// (add) Token: 0x06001FB4 RID: 8116 RVA: 0x000B93E4 File Offset: 0x000B75E4
		// (remove) Token: 0x06001FB5 RID: 8117 RVA: 0x000B941C File Offset: 0x000B761C
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<List<PlayerInfo>> PlayerListChanged;

		// Token: 0x170006F0 RID: 1776
		// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000B9451 File Offset: 0x000B7651
		public IReadOnlyList<PlayerInfo> PlayerList
		{
			get
			{
				if (this._playerList != null)
				{
					return this._playerList.Values.ToList<PlayerInfo>();
				}
				return new List<PlayerInfo>();
			}
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x000B9471 File Offset: 0x000B7671
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeOverlay();
			base.SubscribeNetworkEvent<FullPlayerListEvent>(new EntityEventHandler<FullPlayerListEvent>(this.OnPlayerListChanged), null, null);
			base.SubscribeNetworkEvent<PlayerInfoChangedEvent>(new EntityEventHandler<PlayerInfoChangedEvent>(this.OnPlayerInfoChanged), null, null);
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x000B94A7 File Offset: 0x000B76A7
		public override void Shutdown()
		{
			base.Shutdown();
			this.ShutdownOverlay();
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x000B94B8 File Offset: 0x000B76B8
		private void OnPlayerInfoChanged(PlayerInfoChangedEvent ev)
		{
			if (ev.PlayerInfo == null)
			{
				return;
			}
			if (this._playerList == null)
			{
				this._playerList = new Dictionary<NetUserId, PlayerInfo>();
			}
			this._playerList[ev.PlayerInfo.SessionId] = ev.PlayerInfo;
			Action<List<PlayerInfo>> playerListChanged = this.PlayerListChanged;
			if (playerListChanged == null)
			{
				return;
			}
			playerListChanged(this._playerList.Values.ToList<PlayerInfo>());
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x000B9524 File Offset: 0x000B7724
		private void OnPlayerListChanged(FullPlayerListEvent msg)
		{
			this._playerList = msg.PlayersInfo.ToDictionary((PlayerInfo x) => x.SessionId, (PlayerInfo x) => x);
			Action<List<PlayerInfo>> playerListChanged = this.PlayerListChanged;
			if (playerListChanged == null)
			{
				return;
			}
			playerListChanged(msg.PlayersInfo);
		}

		// Token: 0x140000C0 RID: 192
		// (add) Token: 0x06001FBB RID: 8123 RVA: 0x000B9598 File Offset: 0x000B7798
		// (remove) Token: 0x06001FBC RID: 8124 RVA: 0x000B95D0 File Offset: 0x000B77D0
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action OverlayEnabled;

		// Token: 0x140000C1 RID: 193
		// (add) Token: 0x06001FBD RID: 8125 RVA: 0x000B9608 File Offset: 0x000B7808
		// (remove) Token: 0x06001FBE RID: 8126 RVA: 0x000B9640 File Offset: 0x000B7840
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action OverlayDisabled;

		// Token: 0x06001FBF RID: 8127 RVA: 0x000B9675 File Offset: 0x000B7875
		private void InitializeOverlay()
		{
			this._adminNameOverlay = new AdminNameOverlay(this, this.EntityManager, this._eyeManager, this._resourceCache, this._entityLookup);
			this._adminManager.AdminStatusUpdated += this.OnAdminStatusUpdated;
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x000B96B2 File Offset: 0x000B78B2
		private void ShutdownOverlay()
		{
			this._adminManager.AdminStatusUpdated -= this.OnAdminStatusUpdated;
		}

		// Token: 0x06001FC1 RID: 8129 RVA: 0x000B96CB File Offset: 0x000B78CB
		private void OnAdminStatusUpdated()
		{
			this.AdminOverlayOff();
		}

		// Token: 0x06001FC2 RID: 8130 RVA: 0x000B96D3 File Offset: 0x000B78D3
		public void AdminOverlayOn()
		{
			if (this._overlayManager.HasOverlay<AdminNameOverlay>())
			{
				return;
			}
			this._overlayManager.AddOverlay(this._adminNameOverlay);
			Action overlayEnabled = this.OverlayEnabled;
			if (overlayEnabled == null)
			{
				return;
			}
			overlayEnabled();
		}

		// Token: 0x06001FC3 RID: 8131 RVA: 0x000B9705 File Offset: 0x000B7905
		public void AdminOverlayOff()
		{
			this._overlayManager.RemoveOverlay<AdminNameOverlay>();
			Action overlayDisabled = this.OverlayDisabled;
			if (overlayDisabled == null)
			{
				return;
			}
			overlayDisabled();
		}

		// Token: 0x04000F24 RID: 3876
		[Nullable(new byte[]
		{
			2,
			1
		})]
		private Dictionary<NetUserId, PlayerInfo> _playerList;

		// Token: 0x04000F25 RID: 3877
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x04000F26 RID: 3878
		[Dependency]
		private readonly IResourceCache _resourceCache;

		// Token: 0x04000F27 RID: 3879
		[Dependency]
		private readonly IClientAdminManager _adminManager;

		// Token: 0x04000F28 RID: 3880
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000F29 RID: 3881
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04000F2A RID: 3882
		private AdminNameOverlay _adminNameOverlay;
	}
}
