using System;
using System.Runtime.CompilerServices;
using Content.Shared.Fax;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Fax.UI
{
	// Token: 0x02000318 RID: 792
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FaxBoundUi : BoundUserInterface
	{
		// Token: 0x060013F2 RID: 5106 RVA: 0x000021BC File Offset: 0x000003BC
		public FaxBoundUi(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x00075408 File Offset: 0x00073608
		protected override void Open()
		{
			base.Open();
			this._window = new FaxWindow();
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.SendButtonPressed += this.OnSendButtonPressed;
			this._window.RefreshButtonPressed += this.OnRefreshButtonPressed;
			this._window.PeerSelected += this.OnPeerSelected;
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0007548D File Offset: 0x0007368D
		private void OnSendButtonPressed()
		{
			base.SendMessage(new FaxSendMessage());
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x0007549A File Offset: 0x0007369A
		private void OnRefreshButtonPressed()
		{
			base.SendMessage(new FaxRefreshMessage());
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x000754A7 File Offset: 0x000736A7
		private void OnPeerSelected(string address)
		{
			base.SendMessage(new FaxDestinationMessage(address));
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x000754B8 File Offset: 0x000736B8
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				FaxUiState faxUiState = state as FaxUiState;
				if (faxUiState != null)
				{
					this._window.UpdateState(faxUiState);
					return;
				}
			}
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x000754EB File Offset: 0x000736EB
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				FaxWindow window = this._window;
				if (window == null)
				{
					return;
				}
				window.Dispose();
			}
		}

		// Token: 0x04000A09 RID: 2569
		[Nullable(2)]
		private FaxWindow _window;
	}
}
