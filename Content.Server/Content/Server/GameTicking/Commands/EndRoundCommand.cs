using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D3 RID: 1235
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class EndRoundCommand : IConsoleCommand
	{
		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x00086296 File Offset: 0x00084496
		public string Command
		{
			get
			{
				return "endround";
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x0600197C RID: 6524 RVA: 0x0008629D File Offset: 0x0008449D
		public string Description
		{
			get
			{
				return "Ends the round and moves the server to PostRound.";
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x000862A4 File Offset: 0x000844A4
		public string Help
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x000862AC File Offset: 0x000844AC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			GameTicker ticker = EntitySystem.Get<GameTicker>();
			if (ticker.RunLevel != GameRunLevel.InRound)
			{
				shell.WriteLine("This can only be executed while the game is in a round.");
				return;
			}
			ticker.EndRound("");
		}
	}
}
