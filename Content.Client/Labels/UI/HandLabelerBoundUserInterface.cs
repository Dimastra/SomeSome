using System;
using System.Runtime.CompilerServices;
using Content.Shared.Labels;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Labels.UI
{
	// Token: 0x0200028B RID: 651
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandLabelerBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001092 RID: 4242 RVA: 0x000021BC File Offset: 0x000003BC
		public HandLabelerBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000634F0 File Offset: 0x000616F0
		protected override void Open()
		{
			base.Open();
			this._window = new HandLabelerWindow();
			if (base.State != null)
			{
				this.UpdateState(base.State);
			}
			this._window.OpenCentered();
			this._window.OnClose += base.Close;
			this._window.OnLabelEntered += this.OnLabelChanged;
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x0006355B File Offset: 0x0006175B
		private void OnLabelChanged(string newLabel)
		{
			base.SendMessage(new HandLabelerLabelChangedMessage(newLabel));
			base.Close();
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00063570 File Offset: 0x00061770
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._window != null)
			{
				HandLabelerBoundUserInterfaceState handLabelerBoundUserInterfaceState = state as HandLabelerBoundUserInterfaceState;
				if (handLabelerBoundUserInterfaceState != null)
				{
					this._window.SetCurrentLabel(handLabelerBoundUserInterfaceState.CurrentLabel);
					return;
				}
			}
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x000635A8 File Offset: 0x000617A8
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			HandLabelerWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000833 RID: 2099
		[Nullable(2)]
		private HandLabelerWindow _window;
	}
}
