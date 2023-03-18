using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DA RID: 1242
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class RestartRoundCommand : IConsoleCommand
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x060019A0 RID: 6560 RVA: 0x000867E0 File Offset: 0x000849E0
		public string Command
		{
			get
			{
				return "restartround";
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x060019A1 RID: 6561 RVA: 0x000867E7 File Offset: 0x000849E7
		public string Description
		{
			get
			{
				return "Ends the current round and starts the countdown for the next lobby.";
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x000867EE File Offset: 0x000849EE
		public string Help
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x000867F5 File Offset: 0x000849F5
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (EntitySystem.Get<GameTicker>().RunLevel != GameRunLevel.InRound)
			{
				shell.WriteLine("This can only be executed while the game is in a round - try restartroundnow");
				return;
			}
			EntitySystem.Get<RoundEndSystem>().EndRound();
		}
	}
}
