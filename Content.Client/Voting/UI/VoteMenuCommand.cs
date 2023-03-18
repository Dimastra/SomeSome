using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Client.Voting.UI
{
	// Token: 0x0200004A RID: 74
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class VoteMenuCommand : IConsoleCommand
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000153 RID: 339 RVA: 0x0000B275 File Offset: 0x00009475
		public string Command
		{
			get
			{
				return "votemenu";
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000154 RID: 340 RVA: 0x0000B27C File Offset: 0x0000947C
		public string Description
		{
			get
			{
				return Loc.GetString("ui-vote-menu-command-description");
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000155 RID: 341 RVA: 0x0000B288 File Offset: 0x00009488
		public string Help
		{
			get
			{
				return Loc.GetString("ui-vote-menu-command-help-text");
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000B294 File Offset: 0x00009494
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			new VoteCallMenu().OpenCentered();
		}
	}
}
