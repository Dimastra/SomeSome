using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000837 RID: 2103
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.None)]
	public sealed class DeAdminCommand : IConsoleCommand
	{
		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06002E09 RID: 11785 RVA: 0x000F0AD2 File Offset: 0x000EECD2
		public string Command
		{
			get
			{
				return "deadmin";
			}
		}

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06002E0A RID: 11786 RVA: 0x000F0AD9 File Offset: 0x000EECD9
		public string Description
		{
			get
			{
				return "Temporarily de-admins you so you can experience the round as a normal player.";
			}
		}

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06002E0B RID: 11787 RVA: 0x000F0AE0 File Offset: 0x000EECE0
		public string Help
		{
			get
			{
				return "Usage: deadmin\nUse readmin to re-admin after using this.";
			}
		}

		// Token: 0x06002E0C RID: 11788 RVA: 0x000F0AE8 File Offset: 0x000EECE8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You cannot use this command from the server console.");
				return;
			}
			IoCManager.Resolve<IAdminManager>().DeAdmin(player);
		}
	}
}
