using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink
{
	// Token: 0x020000B5 RID: 181
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class UserAHelpUIHandler : IAHelpUIHandler, IDisposable
	{
		// Token: 0x060004D9 RID: 1241 RVA: 0x0001AF75 File Offset: 0x00019175
		public UserAHelpUIHandler(NetUserId owner)
		{
			this._ownerId = owner;
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x00003C59 File Offset: 0x00001E59
		public bool IsAdmin
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x0001AF84 File Offset: 0x00019184
		public bool IsOpen
		{
			get
			{
				DefaultWindow window = this._window;
				return window != null && !window.Disposed && window.IsOpen;
			}
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001AFAB File Offset: 0x000191AB
		[NullableContext(1)]
		public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
		{
			this.EnsureInit();
			this._chatPanel.ReceiveLine(message);
			this._window.OpenCentered();
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001AFCA File Offset: 0x000191CA
		public void Close()
		{
			DefaultWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Close();
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001AFDC File Offset: 0x000191DC
		public void ToggleWindow()
		{
			this.EnsureInit();
			if (this._window.IsOpen)
			{
				this._window.Close();
				return;
			}
			this._window.OpenCentered();
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001B008 File Offset: 0x00019208
		public void PopOut()
		{
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x060004E0 RID: 1248 RVA: 0x0001B00C File Offset: 0x0001920C
		// (remove) Token: 0x060004E1 RID: 1249 RVA: 0x0001B044 File Offset: 0x00019244
		public event Action OnClose;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060004E2 RID: 1250 RVA: 0x0001B07C File Offset: 0x0001927C
		// (remove) Token: 0x060004E3 RID: 1251 RVA: 0x0001B0B4 File Offset: 0x000192B4
		public event Action OnOpen;

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060004E4 RID: 1252 RVA: 0x0001B0E9 File Offset: 0x000192E9
		// (set) Token: 0x060004E5 RID: 1253 RVA: 0x0001B0F1 File Offset: 0x000192F1
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<NetUserId, string> SendMessageAction { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] set; }

		// Token: 0x060004E6 RID: 1254 RVA: 0x0001B0FA File Offset: 0x000192FA
		public void Open(NetUserId channelId)
		{
			this.EnsureInit();
			this._window.OpenCentered();
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001B110 File Offset: 0x00019310
		private void EnsureInit()
		{
			DefaultWindow window = this._window;
			if (window != null && !window.Disposed)
			{
				return;
			}
			this._chatPanel = new BwoinkPanel(delegate(string text)
			{
				Action<NetUserId, string> sendMessageAction = this.SendMessageAction;
				if (sendMessageAction == null)
				{
					return;
				}
				sendMessageAction(this._ownerId, text);
			});
			this._window = new DefaultWindow
			{
				TitleClass = "windowTitleAlert",
				HeaderClass = "windowHeaderAlert",
				Title = Loc.GetString("bwoink-user-title"),
				SetSize = new ValueTuple<float, float>(400f, 200f)
			};
			this._window.OnClose += delegate()
			{
				Action onClose = this.OnClose;
				if (onClose == null)
				{
					return;
				}
				onClose();
			};
			this._window.OnOpen += delegate()
			{
				Action onOpen = this.OnOpen;
				if (onOpen == null)
				{
					return;
				}
				onOpen();
			};
			this._window.Contents.AddChild(this._chatPanel);
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001B1D6 File Offset: 0x000193D6
		public void Dispose()
		{
			DefaultWindow window = this._window;
			if (window != null)
			{
				window.Dispose();
			}
			this._window = null;
			this._chatPanel = null;
		}

		// Token: 0x04000241 RID: 577
		private readonly NetUserId _ownerId;

		// Token: 0x04000242 RID: 578
		private DefaultWindow _window;

		// Token: 0x04000243 RID: 579
		private BwoinkPanel _chatPanel;
	}
}
