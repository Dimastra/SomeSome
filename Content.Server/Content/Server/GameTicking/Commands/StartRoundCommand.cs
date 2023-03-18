using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DD RID: 1245
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class StartRoundCommand : IConsoleCommand
	{
		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060019AF RID: 6575 RVA: 0x00086979 File Offset: 0x00084B79
		public string Command
		{
			get
			{
				return "startround";
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x00086980 File Offset: 0x00084B80
		public string Description
		{
			get
			{
				return "Ends PreRoundLobby state and starts the round.";
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x00086987 File Offset: 0x00084B87
		public string Help
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x00086990 File Offset: 0x00084B90
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (ticker.RunLevel != GameRunLevel.PreRoundLobby)
			{
				shell.WriteLine("This can only be executed while the game is in the pre-round lobby.");
				return;
			}
			ticker.StartRound(false);
		}
	}
}
