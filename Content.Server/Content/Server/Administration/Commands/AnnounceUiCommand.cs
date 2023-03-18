using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.UI;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000832 RID: 2098
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AnnounceUiCommand : IConsoleCommand
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06002DEF RID: 11759 RVA: 0x000F05D4 File Offset: 0x000EE7D4
		public string Command
		{
			get
			{
				return "announceui";
			}
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06002DF0 RID: 11760 RVA: 0x000F05DB File Offset: 0x000EE7DB
		public string Description
		{
			get
			{
				return "Opens the announcement UI";
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x000F05E2 File Offset: 0x000EE7E2
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06002DF2 RID: 11762 RVA: 0x000F05F4 File Offset: 0x000EE7F4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("This does not work from the server console.");
				return;
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			AdminAnnounceEui ui = new AdminAnnounceEui();
			euiManager.OpenEui(ui, player);
		}
	}
}
