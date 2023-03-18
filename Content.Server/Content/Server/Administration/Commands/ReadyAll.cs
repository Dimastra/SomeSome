using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000855 RID: 2133
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	public sealed class ReadyAll : IConsoleCommand
	{
		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06002EA7 RID: 11943 RVA: 0x000F276B File Offset: 0x000F096B
		public string Command
		{
			get
			{
				return "readyall";
			}
		}

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06002EA8 RID: 11944 RVA: 0x000F2772 File Offset: 0x000F0972
		public string Description
		{
			get
			{
				return "Readies up all players in the lobby, except for observers.";
			}
		}

		// Token: 0x17000789 RID: 1929
		// (get) Token: 0x06002EA9 RID: 11945 RVA: 0x000F2779 File Offset: 0x000F0979
		public string Help
		{
			get
			{
				return this.Command + " | ̣" + this.Command + " <ready>";
			}
		}

		// Token: 0x06002EAA RID: 11946 RVA: 0x000F2798 File Offset: 0x000F0998
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			bool ready = true;
			if (args.Length != 0)
			{
				ready = bool.Parse(args[0]);
			}
			GameTicker gameTicker = EntitySystem.Get<GameTicker>();
			if (gameTicker.RunLevel != GameRunLevel.PreRoundLobby)
			{
				shell.WriteLine("This command can only be ran while in the lobby!");
				return;
			}
			gameTicker.ToggleReadyAll(ready);
		}
	}
}
