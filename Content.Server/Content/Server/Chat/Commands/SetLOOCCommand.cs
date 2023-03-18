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
	// Token: 0x020006D4 RID: 1748
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Server)]
	public sealed class SetLOOCCommand : IConsoleCommand
	{
		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x0600247D RID: 9341 RVA: 0x000BE22A File Offset: 0x000BC42A
		public string Command
		{
			get
			{
				return "setlooc";
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x0600247E RID: 9342 RVA: 0x000BE231 File Offset: 0x000BC431
		public string Description
		{
			get
			{
				return Loc.GetString("set-looc-command-description");
			}
		}

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x0600247F RID: 9343 RVA: 0x000BE23D File Offset: 0x000BC43D
		public string Help
		{
			get
			{
				return Loc.GetString("set-looc-command-help");
			}
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x000BE24C File Offset: 0x000BC44C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IConfigurationManager cfg = IoCManager.Resolve<IConfigurationManager>();
			if (args.Length > 1)
			{
				shell.WriteError(Loc.GetString("set-looc-command-too-many-arguments-error"));
				return;
			}
			bool looc = cfg.GetCVar<bool>(CCVars.LoocEnabled);
			if (args.Length == 0)
			{
				looc = !looc;
			}
			if (args.Length == 1 && !bool.TryParse(args[0], out looc))
			{
				shell.WriteError(Loc.GetString("set-looc-command-invalid-argument-error"));
				return;
			}
			cfg.SetCVar<bool>(CCVars.LoocEnabled, looc, false);
			shell.WriteLine(Loc.GetString(looc ? "set-looc-command-looc-enabled" : "set-looc-command-looc-disabled"));
		}
	}
}
