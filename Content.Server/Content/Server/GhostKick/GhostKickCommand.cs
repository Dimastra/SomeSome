using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.GhostKick
{
	// Token: 0x0200048C RID: 1164
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class GhostKickCommand : IConsoleCommand
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x0007A765 File Offset: 0x00078965
		public string Command
		{
			get
			{
				return "ghostkick";
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x0007A76C File Offset: 0x0007896C
		public string Description
		{
			get
			{
				return "Kick a client from the server as if their network just dropped.";
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001758 RID: 5976 RVA: 0x0007A773 File Offset: 0x00078973
		public string Help
		{
			get
			{
				return "Usage: ghostkick <Player> [Reason]";
			}
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x0007A77C File Offset: 0x0007897C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteError("Need at least one argument");
				return;
			}
			string playerName = args[0];
			string reason = (args.Length > 1) ? args[1] : "Ghost kicked by console";
			IPlayerManager playerManager = IoCManager.Resolve<IPlayerManager>();
			GhostKickManager ghostKick = IoCManager.Resolve<GhostKickManager>();
			IPlayerSession player;
			if (!playerManager.TryGetSessionByUsername(playerName, ref player))
			{
				shell.WriteError("Unable to find player: '" + playerName + "'.");
				return;
			}
			ghostKick.DoDisconnect(player.ConnectedClient, reason);
		}
	}
}
