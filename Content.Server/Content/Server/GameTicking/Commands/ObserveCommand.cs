using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.GameTicking;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D8 RID: 1240
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class ObserveCommand : IConsoleCommand
	{
		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06001996 RID: 6550 RVA: 0x00086683 File Offset: 0x00084883
		public string Command
		{
			get
			{
				return "observe";
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06001997 RID: 6551 RVA: 0x0008668A File Offset: 0x0008488A
		public string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06001998 RID: 6552 RVA: 0x00086691 File Offset: 0x00084891
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x00086698 File Offset: 0x00084898
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				return;
			}
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (ticker.RunLevel == GameRunLevel.PreRoundLobby)
			{
				shell.WriteError("Wait until the round starts.");
				return;
			}
			PlayerGameStatus status;
			if (ticker.PlayerGameStatuses.TryGetValue(player.UserId, out status) && status != PlayerGameStatus.JoinedGame)
			{
				ticker.MakeObserve(player);
				return;
			}
			shell.WriteError(player.Name + " is not in the lobby.   This incident will be reported.");
		}
	}
}
