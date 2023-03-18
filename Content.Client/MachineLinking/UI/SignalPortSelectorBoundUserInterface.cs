using System;
using System.Runtime.CompilerServices;
using Content.Shared.MachineLinking;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.MachineLinking.UI
{
	// Token: 0x02000253 RID: 595
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SignalPortSelectorBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000F16 RID: 3862 RVA: 0x000021BC File Offset: 0x000003BC
		public SignalPortSelectorBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x0005AA32 File Offset: 0x00058C32
		protected override void Open()
		{
			base.Open();
			this._menu = new SignalPortSelectorMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x0005AA68 File Offset: 0x00058C68
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			SignalPortsState signalPortsState = state as SignalPortsState;
			if (signalPortsState != null)
			{
				SignalPortSelectorMenu menu = this._menu;
				if (menu != null)
				{
					menu.UpdateState(signalPortsState);
				}
				this._selectedTransmitterPort = null;
				this._selectedReceiverPort = null;
			}
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x0005AAA6 File Offset: 0x00058CA6
		public void OnTransmitterPortSelected(string port)
		{
			this._selectedTransmitterPort = port;
			if (this._selectedReceiverPort != null)
			{
				base.SendMessage(new SignalPortSelected(this._selectedTransmitterPort, this._selectedReceiverPort));
				this._selectedTransmitterPort = null;
				this._selectedReceiverPort = null;
			}
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x0005AADC File Offset: 0x00058CDC
		public void OnReceiverPortSelected(string port)
		{
			this._selectedReceiverPort = port;
			if (this._selectedTransmitterPort != null)
			{
				base.SendMessage(new SignalPortSelected(this._selectedTransmitterPort, this._selectedReceiverPort));
				this._selectedTransmitterPort = null;
				this._selectedReceiverPort = null;
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0005AB12 File Offset: 0x00058D12
		public void OnClearPressed()
		{
			this._selectedTransmitterPort = null;
			this._selectedReceiverPort = null;
			base.SendMessage(new LinkerClearSelected());
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0005AB2D File Offset: 0x00058D2D
		public void OnLinkDefaultPressed()
		{
			this._selectedTransmitterPort = null;
			this._selectedReceiverPort = null;
			base.SendMessage(new LinkerLinkDefaultSelected());
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x0005AB48 File Offset: 0x00058D48
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			SignalPortSelectorMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000780 RID: 1920
		[Nullable(2)]
		private SignalPortSelectorMenu _menu;

		// Token: 0x04000781 RID: 1921
		[Nullable(2)]
		private string _selectedTransmitterPort;

		// Token: 0x04000782 RID: 1922
		[Nullable(2)]
		private string _selectedReceiverPort;
	}
}
