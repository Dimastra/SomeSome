using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Cloning;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;

namespace Content.Client.Cloning.UI
{
	// Token: 0x020003BD RID: 957
	public sealed class AcceptCloningEui : BaseEui
	{
		// Token: 0x060017CD RID: 6093 RVA: 0x00088EF4 File Offset: 0x000870F4
		public AcceptCloningEui()
		{
			this._window = new AcceptCloningWindow();
			this._window.DenyButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AcceptCloningChoiceMessage(AcceptCloningUiButton.Deny));
				this._window.Close();
			};
			this._window.AcceptButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AcceptCloningChoiceMessage(AcceptCloningUiButton.Accept));
				this._window.Close();
			};
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x00088F4A File Offset: 0x0008714A
		public override void Opened()
		{
			IoCManager.Resolve<IClyde>().RequestWindowAttention();
			this._window.OpenCentered();
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x00088F61 File Offset: 0x00087161
		public override void Closed()
		{
			this._window.Close();
		}

		// Token: 0x04000C1D RID: 3101
		[Nullable(1)]
		private readonly AcceptCloningWindow _window;
	}
}
