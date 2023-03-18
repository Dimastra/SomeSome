using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Client.Administration.UI.Bwoink;
using Content.Shared.Administration;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Network;

namespace Content.Client.UserInterface.Systems.Bwoink
{
	// Token: 0x020000B3 RID: 179
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class AdminAHelpUIHandler : IAHelpUIHandler, IDisposable
	{
		// Token: 0x060004C1 RID: 1217 RVA: 0x0001AABA File Offset: 0x00018CBA
		public AdminAHelpUIHandler(NetUserId owner)
		{
			this._ownerId = owner;
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x00003C56 File Offset: 0x00001E56
		public bool IsAdmin
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x0001AAD4 File Offset: 0x00018CD4
		public bool IsOpen
		{
			get
			{
				BwoinkWindow window = this.Window;
				if (window == null || window.Disposed || !window.IsOpen)
				{
					IClydeWindow clydeWindow = this.ClydeWindow;
					return clydeWindow != null && !clydeWindow.IsDisposed;
				}
				return true;
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001AB12 File Offset: 0x00018D12
		[NullableContext(1)]
		public void Receive(SharedBwoinkSystem.BwoinkTextMessage message)
		{
			this.EnsurePanel(message.UserId).ReceiveLine(message);
			BwoinkControl control = this.Control;
			if (control == null)
			{
				return;
			}
			control.OnBwoink(message.UserId);
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001AB3C File Offset: 0x00018D3C
		private void OpenWindow()
		{
			if (this.Window == null)
			{
				return;
			}
			if (this.EverOpened)
			{
				this.Window.Open();
				return;
			}
			this.Window.OpenCentered();
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001AB68 File Offset: 0x00018D68
		public void Close()
		{
			BwoinkWindow window = this.Window;
			if (window != null)
			{
				window.Close();
			}
			if (this.ClydeWindow != null)
			{
				this.ClydeWindow.RequestClosed -= this.OnRequestClosed;
				this.ClydeWindow.Dispose();
				if (this.Control != null)
				{
					foreach (KeyValuePair<NetUserId, BwoinkPanel> keyValuePair in this._activePanelMap)
					{
						NetUserId netUserId;
						BwoinkPanel bwoinkPanel;
						keyValuePair.Deconstruct(out netUserId, out bwoinkPanel);
						bwoinkPanel.Orphan();
					}
					BwoinkControl control = this.Control;
					if (control != null)
					{
						control.Dispose();
					}
				}
				Action onClose = this.OnClose;
				if (onClose == null)
				{
					return;
				}
				onClose();
			}
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001AC2C File Offset: 0x00018E2C
		public void ToggleWindow()
		{
			this.EnsurePanel(this._ownerId);
			if (this.IsOpen)
			{
				this.Close();
				return;
			}
			this.OpenWindow();
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060004C8 RID: 1224 RVA: 0x0001AC50 File Offset: 0x00018E50
		// (remove) Token: 0x060004C9 RID: 1225 RVA: 0x0001AC88 File Offset: 0x00018E88
		public event Action OnClose;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x060004CA RID: 1226 RVA: 0x0001ACC0 File Offset: 0x00018EC0
		// (remove) Token: 0x060004CB RID: 1227 RVA: 0x0001ACF8 File Offset: 0x00018EF8
		public event Action OnOpen;

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001AD2D File Offset: 0x00018F2D
		// (set) Token: 0x060004CD RID: 1229 RVA: 0x0001AD35 File Offset: 0x00018F35
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

		// Token: 0x060004CE RID: 1230 RVA: 0x0001AD3E File Offset: 0x00018F3E
		public void Open(NetUserId channelId)
		{
			this.SelectChannel(channelId);
			this.OpenWindow();
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001AD4D File Offset: 0x00018F4D
		public void OnRequestClosed(WindowRequestClosedEventArgs args)
		{
			this.Close();
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001AD58 File Offset: 0x00018F58
		private void EnsureControl()
		{
			BwoinkControl control = this.Control;
			if (control != null && !control.Disposed)
			{
				return;
			}
			this.Window = new BwoinkWindow();
			this.Control = this.Window.Bwoink;
			this.Window.OnClose += delegate()
			{
				Action onClose = this.OnClose;
				if (onClose == null)
				{
					return;
				}
				onClose();
			};
			this.Window.OnOpen += delegate()
			{
				Action onOpen = this.OnOpen;
				if (onOpen != null)
				{
					onOpen();
				}
				this.EverOpened = true;
			};
			foreach (KeyValuePair<NetUserId, BwoinkPanel> keyValuePair in this._activePanelMap)
			{
				NetUserId netUserId;
				BwoinkPanel bwoinkPanel;
				keyValuePair.Deconstruct(out netUserId, out bwoinkPanel);
				BwoinkPanel bwoinkPanel2 = bwoinkPanel;
				if (!this.Control.BwoinkArea.Children.Contains(bwoinkPanel2))
				{
					this.Control.BwoinkArea.AddChild(bwoinkPanel2);
				}
				bwoinkPanel2.Visible = false;
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001AE40 File Offset: 0x00019040
		[NullableContext(1)]
		public BwoinkPanel EnsurePanel(NetUserId channelId)
		{
			this.EnsureControl();
			BwoinkPanel bwoinkPanel;
			if (this._activePanelMap.TryGetValue(channelId, out bwoinkPanel))
			{
				return bwoinkPanel;
			}
			bwoinkPanel = (this._activePanelMap[channelId] = new BwoinkPanel(delegate(string text)
			{
				Action<NetUserId, string> sendMessageAction = this.SendMessageAction;
				if (sendMessageAction == null)
				{
					return;
				}
				sendMessageAction(channelId, text);
			}));
			bwoinkPanel.Visible = false;
			if (!this.Control.BwoinkArea.Children.Contains(bwoinkPanel))
			{
				this.Control.BwoinkArea.AddChild(bwoinkPanel);
			}
			return bwoinkPanel;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001AED3 File Offset: 0x000190D3
		public bool TryGetChannel(NetUserId ch, [NotNullWhen(true)] out BwoinkPanel bp)
		{
			return this._activePanelMap.TryGetValue(ch, out bp);
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001AEE2 File Offset: 0x000190E2
		private void SelectChannel(NetUserId uid)
		{
			this.EnsurePanel(uid);
			this.Control.SelectChannel(uid);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001AEF8 File Offset: 0x000190F8
		public void Dispose()
		{
			BwoinkWindow window = this.Window;
			if (window != null)
			{
				window.Dispose();
			}
			this.Window = null;
			this.Control = null;
			this._activePanelMap.Clear();
			this.EverOpened = false;
		}

		// Token: 0x04000235 RID: 565
		private readonly NetUserId _ownerId;

		// Token: 0x04000236 RID: 566
		[Nullable(1)]
		private readonly Dictionary<NetUserId, BwoinkPanel> _activePanelMap = new Dictionary<NetUserId, BwoinkPanel>();

		// Token: 0x04000237 RID: 567
		public bool EverOpened;

		// Token: 0x04000238 RID: 568
		public BwoinkWindow Window;

		// Token: 0x04000239 RID: 569
		public WindowRoot WindowRoot;

		// Token: 0x0400023A RID: 570
		public IClydeWindow ClydeWindow;

		// Token: 0x0400023B RID: 571
		public BwoinkControl Control;
	}
}
