using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000854 RID: 2132
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ReAdminCommand : IConsoleCommand
	{
		// Token: 0x17000784 RID: 1924
		// (get) Token: 0x06002EA2 RID: 11938 RVA: 0x000F2703 File Offset: 0x000F0903
		public string Command
		{
			get
			{
				return "readmin";
			}
		}

		// Token: 0x17000785 RID: 1925
		// (get) Token: 0x06002EA3 RID: 11939 RVA: 0x000F270A File Offset: 0x000F090A
		public string Description
		{
			get
			{
				return "Re-admins you if you previously de-adminned.";
			}
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06002EA4 RID: 11940 RVA: 0x000F2711 File Offset: 0x000F0911
		public string Help
		{
			get
			{
				return "Usage: readmin";
			}
		}

		// Token: 0x06002EA5 RID: 11941 RVA: 0x000F2718 File Offset: 0x000F0918
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You cannot use this command from the server console.");
				return;
			}
			IAdminManager mgr = IoCManager.Resolve<IAdminManager>();
			if (mgr.GetAdminData(player, true) == null)
			{
				shell.WriteLine("You're not an admin.");
				return;
			}
			mgr.ReAdmin(player);
		}
	}
}
