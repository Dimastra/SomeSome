using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DB RID: 1243
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class RestartRoundNowCommand : IConsoleCommand
	{
		// Token: 0x170003CC RID: 972
		// (get) Token: 0x060019A5 RID: 6565 RVA: 0x00086822 File Offset: 0x00084A22
		public string Command
		{
			get
			{
				return "restartroundnow";
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x060019A6 RID: 6566 RVA: 0x00086829 File Offset: 0x00084A29
		public string Description
		{
			get
			{
				return "Moves the server from PostRound to a new PreRoundLobby.";
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060019A7 RID: 6567 RVA: 0x00086830 File Offset: 0x00084A30
		public string Help
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x00086837 File Offset: 0x00084A37
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			EntitySystem.Get<GameTicker>().RestartRound();
		}
	}
}
