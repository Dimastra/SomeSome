using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006D5 RID: 1749
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Server)]
	public sealed class SetOOCCommand : IConsoleCommand
	{
		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06002482 RID: 9346 RVA: 0x000BE2DD File Offset: 0x000BC4DD
		public string Command
		{
			get
			{
				return "setooc";
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06002483 RID: 9347 RVA: 0x000BE2E4 File Offset: 0x000BC4E4
		public string Description
		{
			get
			{
				return Loc.GetString("set-ooc-command-description");
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06002484 RID: 9348 RVA: 0x000BE2F0 File Offset: 0x000BC4F0
		public string Help
		{
			get
			{
				return Loc.GetString("set-ooc-command-help");
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x000BE2FC File Offset: 0x000BC4FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IConfigurationManager cfg = IoCManager.Resolve<IConfigurationManager>();
			if (args.Length > 1)
			{
				shell.WriteError(Loc.GetString("set-ooc-command-too-many-arguments-error"));
				return;
			}
			bool ooc = cfg.GetCVar<bool>(CCVars.OocEnabled);
			if (args.Length == 0)
			{
				ooc = !ooc;
			}
			if (args.Length == 1 && !bool.TryParse(args[0], out ooc))
			{
				shell.WriteError(Loc.GetString("set-ooc-command-invalid-argument-error"));
				return;
			}
			cfg.SetCVar<bool>(CCVars.OocEnabled, ooc, false);
			shell.WriteLine(Loc.GetString(ooc ? "set-ooc-command-ooc-enabled" : "set-ooc-command-ooc-disabled"));
		}
	}
}
