using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cloning.CloningConsole;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Client.CloningConsole.UI
{
	// Token: 0x020003BB RID: 955
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CloningConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060017B6 RID: 6070 RVA: 0x000021BC File Offset: 0x000003BC
		public CloningConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x00088158 File Offset: 0x00086358
		protected override void Open()
		{
			base.Open();
			this._window = new CloningConsoleWindow
			{
				Title = Loc.GetString("cloning-console-window-title")
			};
			this._window.OnClose += base.Close;
			this._window.CloneButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new UiButtonPressedMessage(UiButton.Clone));
			};
			this._window.OpenCentered();
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x000881C4 File Offset: 0x000863C4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CloningConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Populate((CloningConsoleBoundUserInterfaceState)state);
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x000881E4 File Offset: 0x000863E4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			if (this._window != null)
			{
				this._window.OnClose -= base.Close;
				this._window.CloneButton.OnPressed -= delegate(BaseButton.ButtonEventArgs _)
				{
					base.SendMessage(new UiButtonPressedMessage(UiButton.Clone));
				};
			}
			CloningConsoleWindow window = this._window;
			if (window == null)
			{
				return;
			}
			window.Dispose();
		}

		// Token: 0x04000C1B RID: 3099
		[Nullable(2)]
		private CloningConsoleWindow _window;
	}
}
