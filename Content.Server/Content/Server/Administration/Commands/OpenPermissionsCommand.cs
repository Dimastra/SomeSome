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
	// Token: 0x02000847 RID: 2119
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Permissions)]
	public sealed class OpenPermissionsCommand : IConsoleCommand
	{
		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06002E5C RID: 11868 RVA: 0x000F1CE3 File Offset: 0x000EFEE3
		public string Command
		{
			get
			{
				return "permissions";
			}
		}

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06002E5D RID: 11869 RVA: 0x000F1CEA File Offset: 0x000EFEEA
		public string Description
		{
			get
			{
				return "Opens the admin permissions panel.";
			}
		}

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06002E5E RID: 11870 RVA: 0x000F1CF1 File Offset: 0x000EFEF1
		public string Help
		{
			get
			{
				return "Usage: permissions";
			}
		}

		// Token: 0x06002E5F RID: 11871 RVA: 0x000F1CF8 File Offset: 0x000EFEF8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("This does not work from the server console.");
				return;
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			PermissionsEui ui = new PermissionsEui();
			euiManager.OpenEui(ui, player);
		}
	}
}
