using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200084A RID: 2122
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Server)]
	public sealed class PanicBunkerCommand : IConsoleCommand
	{
		// Token: 0x17000766 RID: 1894
		// (get) Token: 0x06002E6A RID: 11882 RVA: 0x000F1FA2 File Offset: 0x000F01A2
		public string Command
		{
			get
			{
				return "panicbunker";
			}
		}

		// Token: 0x17000767 RID: 1895
		// (get) Token: 0x06002E6B RID: 11883 RVA: 0x000F1FA9 File Offset: 0x000F01A9
		public string Description
		{
			get
			{
				return "Enables or disables the panic bunker functionality.";
			}
		}

		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06002E6C RID: 11884 RVA: 0x000F1FB0 File Offset: 0x000F01B0
		public string Help
		{
			get
			{
				return "panicbunker";
			}
		}

		// Token: 0x06002E6D RID: 11885 RVA: 0x000F1FB8 File Offset: 0x000F01B8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length > 1)
			{
				shell.WriteError(Loc.GetString("shell-need-between-arguments", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("lower", 0),
					new ValueTuple<string, object>("upper", 1)
				}));
				return;
			}
			bool enabled = this._cfg.GetCVar<bool>(CCVars.PanicBunkerEnabled);
			if (args.Length == 0)
			{
				enabled = !enabled;
			}
			if (args.Length == 1 && !bool.TryParse(args[0], out enabled))
			{
				shell.WriteError(Loc.GetString("shell-argument-must-be-boolean"));
				return;
			}
			this._cfg.SetCVar<bool>(CCVars.PanicBunkerEnabled, enabled, false);
			shell.WriteLine(Loc.GetString(enabled ? "panicbunker-command-enabled" : "panicbunker-command-disabled"));
		}

		// Token: 0x04001C53 RID: 7251
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
