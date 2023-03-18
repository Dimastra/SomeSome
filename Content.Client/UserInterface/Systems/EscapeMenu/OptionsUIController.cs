using System;
using System.Runtime.CompilerServices;
using Content.Client.Options.UI;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.EscapeMenu
{
	// Token: 0x02000097 RID: 151
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class OptionsUIController : UIController
	{
		// Token: 0x06000397 RID: 919 RVA: 0x00015774 File Offset: 0x00013974
		public override void Initialize()
		{
			this._con.RegisterCommand("options", Loc.GetString("cmd-options-desc"), Loc.GetString("cmd-options-help"), new ConCommandCallback(this.OptionsCommand), false);
		}

		// Token: 0x06000398 RID: 920 RVA: 0x000157A8 File Offset: 0x000139A8
		private void OptionsCommand(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length == 0)
			{
				this.ToggleWindow();
				return;
			}
			this.OpenWindow();
			int currentTab;
			if (!int.TryParse(args[0], out currentTab))
			{
				shell.WriteError(Loc.GetString("cmd-parse-failure-int", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[0])
				}));
				return;
			}
			this._optionsWindow.Tabs.CurrentTab = currentTab;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00015810 File Offset: 0x00013A10
		private void EnsureWindow()
		{
			OptionsMenu optionsWindow = this._optionsWindow;
			if (optionsWindow != null && !optionsWindow.Disposed)
			{
				return;
			}
			this._optionsWindow = this.UIManager.CreateWindow<OptionsMenu>();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00015841 File Offset: 0x00013A41
		public void OpenWindow()
		{
			this.EnsureWindow();
			this._optionsWindow.OpenCentered();
			this._optionsWindow.MoveToFront();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0001585F File Offset: 0x00013A5F
		public void ToggleWindow()
		{
			this.EnsureWindow();
			if (this._optionsWindow.IsOpen)
			{
				this._optionsWindow.Close();
				return;
			}
			this.OpenWindow();
		}

		// Token: 0x040001B3 RID: 435
		[Dependency]
		private readonly IConsoleHost _con;

		// Token: 0x040001B4 RID: 436
		private OptionsMenu _optionsWindow;
	}
}
