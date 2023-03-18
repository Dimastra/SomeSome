using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DE RID: 1246
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class ToggleDisallowLateJoinCommand : IConsoleCommand
	{
		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x000869C6 File Offset: 0x00084BC6
		public string Command
		{
			get
			{
				return "toggledisallowlatejoin";
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x000869CD File Offset: 0x00084BCD
		public string Description
		{
			get
			{
				return "Allows or disallows latejoining during mid-game.";
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x060019B6 RID: 6582 RVA: 0x000869D4 File Offset: 0x00084BD4
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <disallow>";
			}
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x000869EC File Offset: 0x00084BEC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Need exactly one argument.");
				return;
			}
			IConfigurationManager cfgMan = IoCManager.Resolve<IConfigurationManager>();
			bool result;
			if (bool.TryParse(args[0], out result))
			{
				cfgMan.SetCVar<bool>(CCVars.GameDisallowLateJoins, bool.Parse(args[0]), false);
				shell.WriteLine(result ? "Late joining has been disabled." : "Late joining has been enabled.");
				return;
			}
			shell.WriteLine("Invalid argument.");
		}
	}
}
