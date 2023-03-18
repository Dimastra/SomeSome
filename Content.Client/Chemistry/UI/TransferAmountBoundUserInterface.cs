using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003DE RID: 990
	public sealed class TransferAmountBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001870 RID: 6256 RVA: 0x0008D344 File Offset: 0x0008B544
		protected override void Open()
		{
			base.Open();
			this._window = new TransferAmountWindow();
			this._window.ApplyButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				int value;
				if (int.TryParse(this._window.AmountLineEdit.Text, out value))
				{
					base.SendMessage(new TransferAmountSetValueMessage(FixedPoint2.New(value)));
					this._window.Close();
				}
			};
			this._window.OnClose += base.Close;
			this._window.OpenCentered();
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000021BC File Offset: 0x000003BC
		[NullableContext(1)]
		public TransferAmountBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0008D3A0 File Offset: 0x0008B5A0
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			TransferAmountWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000C70 RID: 3184
		[Nullable(2)]
		private TransferAmountWindow _window;
	}
}
