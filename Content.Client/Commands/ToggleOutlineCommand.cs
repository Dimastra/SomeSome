using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003B0 RID: 944
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ToggleOutlineCommand : IConsoleCommand
	{
		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x00086CE7 File Offset: 0x00084EE7
		public string Command
		{
			get
			{
				return "toggleoutline";
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x00086CEE File Offset: 0x00084EEE
		public string Description
		{
			get
			{
				return "Toggles outline drawing on entities.";
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001776 RID: 6006 RVA: 0x00054CC5 File Offset: 0x00052EC5
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x00086CF8 File Offset: 0x00084EF8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			CVarDef<bool> outlineEnabled = CCVars.OutlineEnabled;
			bool cvar = configurationManager.GetCVar<bool>(outlineEnabled);
			configurationManager.SetCVar<bool>(outlineEnabled, !cvar, false);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Draw outlines set to: ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(configurationManager.GetCVar<bool>(outlineEnabled));
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
