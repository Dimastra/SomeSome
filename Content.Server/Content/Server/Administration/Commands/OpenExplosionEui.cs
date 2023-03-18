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
	// Token: 0x0200083F RID: 2111
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class OpenExplosionEui : IConsoleCommand
	{
		// Token: 0x17000747 RID: 1863
		// (get) Token: 0x06002E33 RID: 11827 RVA: 0x000F1307 File Offset: 0x000EF507
		public string Command
		{
			get
			{
				return "explosionui";
			}
		}

		// Token: 0x17000748 RID: 1864
		// (get) Token: 0x06002E34 RID: 11828 RVA: 0x000F130E File Offset: 0x000EF50E
		public string Description
		{
			get
			{
				return "Opens a window for easy access to station destruction";
			}
		}

		// Token: 0x17000749 RID: 1865
		// (get) Token: 0x06002E35 RID: 11829 RVA: 0x000F1315 File Offset: 0x000EF515
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06002E36 RID: 11830 RVA: 0x000F1328 File Offset: 0x000EF528
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError("This does not work from the server console.");
				return;
			}
			EuiManager euiManager = IoCManager.Resolve<EuiManager>();
			SpawnExplosionEui ui = new SpawnExplosionEui();
			euiManager.OpenEui(ui, player);
		}
	}
}
