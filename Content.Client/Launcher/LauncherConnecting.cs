using System;
using System.Runtime.CompilerServices;
using Robust.Client;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Client.Launcher
{
	// Token: 0x0200027A RID: 634
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class LauncherConnecting : State
	{
		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x00060353 File Offset: 0x0005E553
		public string Address
		{
			get
			{
				return this._gameController.LaunchState.Ss14Address ?? this._gameController.LaunchState.ConnectAddress;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x0600101A RID: 4122 RVA: 0x00060379 File Offset: 0x0005E579
		// (set) Token: 0x0600101B RID: 4123 RVA: 0x00060381 File Offset: 0x0005E581
		public string ConnectFailReason
		{
			get
			{
				return this._connectFailReason;
			}
			private set
			{
				this._connectFailReason = value;
				Action<string> connectFailReasonChanged = this.ConnectFailReasonChanged;
				if (connectFailReasonChanged == null)
				{
					return;
				}
				connectFailReasonChanged(value);
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x0600101C RID: 4124 RVA: 0x0006039B File Offset: 0x0005E59B
		public string LastDisconnectReason
		{
			get
			{
				return this._baseClient.LastDisconnectReason;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x0600101D RID: 4125 RVA: 0x000603A8 File Offset: 0x0005E5A8
		// (set) Token: 0x0600101E RID: 4126 RVA: 0x000603B0 File Offset: 0x0005E5B0
		public LauncherConnecting.Page CurrentPage
		{
			get
			{
				return this._currentPage;
			}
			private set
			{
				this._currentPage = value;
				Action<LauncherConnecting.Page> pageChanged = this.PageChanged;
				if (pageChanged == null)
				{
					return;
				}
				pageChanged(value);
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x0600101F RID: 4127 RVA: 0x000603CA File Offset: 0x0005E5CA
		public ClientConnectionState ConnectionState
		{
			get
			{
				return this._clientNetManager.ClientConnectState;
			}
		}

		// Token: 0x14000058 RID: 88
		// (add) Token: 0x06001020 RID: 4128 RVA: 0x000603D8 File Offset: 0x0005E5D8
		// (remove) Token: 0x06001021 RID: 4129 RVA: 0x00060410 File Offset: 0x0005E610
		public event Action<LauncherConnecting.Page> PageChanged;

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x06001022 RID: 4130 RVA: 0x00060448 File Offset: 0x0005E648
		// (remove) Token: 0x06001023 RID: 4131 RVA: 0x00060480 File Offset: 0x0005E680
		public event Action<string> ConnectFailReasonChanged;

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06001024 RID: 4132 RVA: 0x000604B8 File Offset: 0x0005E6B8
		// (remove) Token: 0x06001025 RID: 4133 RVA: 0x000604F0 File Offset: 0x0005E6F0
		public event Action<ClientConnectionState> ConnectionStateChanged;

		// Token: 0x06001026 RID: 4134 RVA: 0x00060528 File Offset: 0x0005E728
		protected override void Startup()
		{
			this._control = new LauncherConnectingGui(this);
			this._userInterfaceManager.StateRoot.AddChild(this._control);
			this._clientNetManager.ConnectFailed += this.OnConnectFailed;
			this._clientNetManager.ClientConnectStateChanged += this.OnConnectStateChanged;
			this.CurrentPage = LauncherConnecting.Page.Connecting;
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0006058C File Offset: 0x0005E78C
		protected override void Shutdown()
		{
			LauncherConnectingGui control = this._control;
			if (control != null)
			{
				control.Dispose();
			}
			this._clientNetManager.ConnectFailed -= this.OnConnectFailed;
			this._clientNetManager.ClientConnectStateChanged -= this.OnConnectStateChanged;
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x000605D8 File Offset: 0x0005E7D8
		[NullableContext(1)]
		private void OnConnectFailed([Nullable(2)] object _, NetConnectFailArgs args)
		{
			if (args.RedialFlag)
			{
				this.Redial();
			}
			this.ConnectFailReason = args.Reason;
			this.CurrentPage = LauncherConnecting.Page.ConnectFailed;
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x000605FC File Offset: 0x0005E7FC
		private void OnConnectStateChanged(ClientConnectionState state)
		{
			Action<ClientConnectionState> connectionStateChanged = this.ConnectionStateChanged;
			if (connectionStateChanged == null)
			{
				return;
			}
			connectionStateChanged(state);
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x0006060F File Offset: 0x0005E80F
		public void RetryConnect()
		{
			if (this._gameController.LaunchState.ConnectEndpoint != null)
			{
				this._baseClient.ConnectToServer(this._gameController.LaunchState.ConnectEndpoint);
				this.CurrentPage = LauncherConnecting.Page.Connecting;
			}
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x00060648 File Offset: 0x0005E848
		public bool Redial()
		{
			try
			{
				if (this._gameController.LaunchState.Ss14Address != null)
				{
					this._gameController.Redial(this._gameController.LaunchState.Ss14Address, null);
					return true;
				}
				Logger.InfoS("launcher-ui", "Redial not possible, no Ss14Address");
			}
			catch (Exception value)
			{
				string text = "launcher-ui";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Redial exception: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(value);
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return false;
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x000606E0 File Offset: 0x0005E8E0
		public void Exit()
		{
			this._gameController.Shutdown("Exit button pressed");
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x000606F2 File Offset: 0x0005E8F2
		public void SetDisconnected()
		{
			this.CurrentPage = LauncherConnecting.Page.Disconnected;
		}

		// Token: 0x040007F5 RID: 2037
		[Nullable(1)]
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x040007F6 RID: 2038
		[Nullable(1)]
		[Dependency]
		private readonly IClientNetManager _clientNetManager;

		// Token: 0x040007F7 RID: 2039
		[Nullable(1)]
		[Dependency]
		private readonly IGameController _gameController;

		// Token: 0x040007F8 RID: 2040
		[Nullable(1)]
		[Dependency]
		private readonly IBaseClient _baseClient;

		// Token: 0x040007F9 RID: 2041
		private LauncherConnectingGui _control;

		// Token: 0x040007FA RID: 2042
		private LauncherConnecting.Page _currentPage;

		// Token: 0x040007FB RID: 2043
		private string _connectFailReason;

		// Token: 0x0200027B RID: 635
		[NullableContext(0)]
		public enum Page : byte
		{
			// Token: 0x04000800 RID: 2048
			Connecting,
			// Token: 0x04000801 RID: 2049
			ConnectFailed,
			// Token: 0x04000802 RID: 2050
			Disconnected
		}
	}
}
