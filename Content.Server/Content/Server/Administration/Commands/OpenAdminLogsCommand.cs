using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000845 RID: 2117
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Logs)]
	public sealed class OpenAdminLogsCommand : IConsoleCommand
	{
		// Token: 0x17000759 RID: 1881
		// (get) Token: 0x06002E52 RID: 11858 RVA: 0x000F1C0C File Offset: 0x000EFE0C
		public string Command
		{
			get
			{
				return "adminlogs";
			}
		}

		// Token: 0x1700075A RID: 1882
		// (get) Token: 0x06002E53 RID: 11859 RVA: 0x000F1C13 File Offset: 0x000EFE13
		public string Description
		{
			get
			{
				return "Opens the admin logs panel.";
			}
		}

		// Token: 0x1700075B RID: 1883
		// (get) Token: 0x06002E54 RID: 11860 RVA: 0x000F1C1A File Offset: 0x000EFE1A
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06002E55 RID: 11861 RVA: 0x000F1C2C File Offset: 0x000EFE2C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("This does not work from the server console.");
				return;
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			AdminLogsEui ui = new AdminLogsEui();
			euiManager.OpenEui(ui, player);
		}
	}
}
