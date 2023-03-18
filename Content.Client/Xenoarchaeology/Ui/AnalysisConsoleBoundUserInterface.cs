using System;
using System.Runtime.CompilerServices;
using Content.Shared.Xenoarchaeology.Equipment;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.Xenoarchaeology.Ui
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AnalysisConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000021BC File Offset: 0x000003BC
		public AnalysisConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021C8 File Offset: 0x000003C8
		protected override void Open()
		{
			base.Open();
			this._consoleMenu = new AnalysisConsoleMenu();
			this._consoleMenu.OnClose += base.Close;
			this._consoleMenu.OpenCentered();
			this._consoleMenu.OnServerSelectionButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AnalysisConsoleServerSelectionMessage());
			};
			this._consoleMenu.OnScanButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AnalysisConsoleScanButtonPressedMessage());
			};
			this._consoleMenu.OnPrintButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AnalysisConsolePrintButtonPressedMessage());
			};
			this._consoleMenu.OnDestroyButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new AnalysisConsoleDestroyButtonPressedMessage());
			};
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002264 File Offset: 0x00000464
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			AnalysisConsoleScanUpdateState analysisConsoleScanUpdateState = state as AnalysisConsoleScanUpdateState;
			if (analysisConsoleScanUpdateState != null)
			{
				AnalysisConsoleMenu consoleMenu = this._consoleMenu;
				if (consoleMenu != null)
				{
					consoleMenu.SetButtonsDisabled(analysisConsoleScanUpdateState);
				}
				AnalysisConsoleMenu consoleMenu2 = this._consoleMenu;
				if (consoleMenu2 != null)
				{
					consoleMenu2.UpdateInformationDisplay(analysisConsoleScanUpdateState);
				}
				AnalysisConsoleMenu consoleMenu3 = this._consoleMenu;
				if (consoleMenu3 == null)
				{
					return;
				}
				consoleMenu3.UpdateProgressBar(analysisConsoleScanUpdateState);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022B7 File Offset: 0x000004B7
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			AnalysisConsoleMenu consoleMenu = this._consoleMenu;
			if (consoleMenu != null)
			{
				AnalysisDestroyWindow analysisDestroyWindow = consoleMenu.AnalysisDestroyWindow;
				if (analysisDestroyWindow != null)
				{
					analysisDestroyWindow.Close();
				}
			}
			AnalysisConsoleMenu consoleMenu2 = this._consoleMenu;
			if (consoleMenu2 == null)
			{
				return;
			}
			consoleMenu2.Dispose();
		}

		// Token: 0x0400000F RID: 15
		[Nullable(2)]
		private AnalysisConsoleMenu _consoleMenu;
	}
}
