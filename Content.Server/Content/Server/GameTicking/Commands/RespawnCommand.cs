using System;
using System.Runtime.CompilerServices;
using Content.Server.Players;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D9 RID: 1241
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class RespawnCommand : IConsoleCommand
	{
		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x0600199B RID: 6555 RVA: 0x0008670E File Offset: 0x0008490E
		public string Command
		{
			get
			{
				return "respawn";
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x0600199C RID: 6556 RVA: 0x00086715 File Offset: 0x00084915
		public string Description
		{
			get
			{
				return "Respawns a player, kicking them back to the lobby.";
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x0600199D RID: 6557 RVA: 0x0008671C File Offset: 0x0008491C
		public string Help
		{
			get
			{
				return "respawn [player]";
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x00086724 File Offset: 0x00084924
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (args.Length > 1)
			{
				shell.WriteLine("Must provide <= 1 argument.");
				return;
			}
			IPlayerManager playerMgr = IoCManager.Resolve<IPlayerManager>();
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			NetUserId userId;
			if (args.Length == 0)
			{
				if (player == null)
				{
					shell.WriteLine("If not a player, an argument must be given.");
					return;
				}
				userId = player.UserId;
			}
			else if (!playerMgr.TryGetUserId(args[0], ref userId))
			{
				shell.WriteLine("Unknown player");
				return;
			}
			IPlayerSession targetPlayer;
			if (playerMgr.TryGetSessionById(userId, ref targetPlayer))
			{
				ticker.Respawn(targetPlayer);
				return;
			}
			IPlayerData data;
			if (!playerMgr.TryGetPlayerData(userId, ref data))
			{
				shell.WriteLine("Unknown player");
				return;
			}
			PlayerData playerData = data.ContentData();
			if (playerData != null)
			{
				playerData.WipeMind();
			}
			shell.WriteLine("Player is not currently online, but they will respawn if they come back online");
		}
	}
}
