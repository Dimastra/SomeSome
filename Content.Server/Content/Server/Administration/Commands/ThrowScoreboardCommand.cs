using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200086A RID: 2154
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.VarEdit)]
	public sealed class ThrowScoreboardCommand : IConsoleCommand
	{
		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x06002F17 RID: 12055 RVA: 0x000F3F15 File Offset: 0x000F2115
		public string Command
		{
			get
			{
				return "throwscoreboard";
			}
		}

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x06002F18 RID: 12056 RVA: 0x000F3F1C File Offset: 0x000F211C
		public string Description
		{
			get
			{
				return Loc.GetString("throw-scoreboard-command-description");
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x06002F19 RID: 12057 RVA: 0x000F3F28 File Offset: 0x000F2128
		public string Help
		{
			get
			{
				return Loc.GetString("throw-scoreboard-command-help-text");
			}
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x000F3F34 File Offset: 0x000F2134
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 0)
			{
				shell.WriteLine(this.Help);
				return;
			}
			EntitySystem.Get<GameTicker>().ShowRoundEndScoreboard("");
		}
	}
}
