using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000853 RID: 2131
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PromoteHostCommand : IConsoleCommand
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06002E9D RID: 11933 RVA: 0x000F269D File Offset: 0x000F089D
		public string Command
		{
			get
			{
				return "promotehost";
			}
		}

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06002E9E RID: 11934 RVA: 0x000F26A4 File Offset: 0x000F08A4
		public string Description
		{
			get
			{
				return "Grants client temporary full host admin privileges. Use this to bootstrap admins.";
			}
		}

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x06002E9F RID: 11935 RVA: 0x000F26AB File Offset: 0x000F08AB
		public string Help
		{
			get
			{
				return "Usage promotehost <player>";
			}
		}

		// Token: 0x06002EA0 RID: 11936 RVA: 0x000F26B4 File Offset: 0x000F08B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Expected exactly one argument.");
				return;
			}
			IPlayerSession targetPlayer;
			if (!IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], ref targetPlayer))
			{
				shell.WriteLine("Unable to find a player by that name.");
				return;
			}
			IoCManager.Resolve<IAdminManager>().PromoteHost(targetPlayer);
		}
	}
}
