using System;
using System.Runtime.CompilerServices;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Voting
{
	// Token: 0x020000C2 RID: 194
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ListVotesCommand : IConsoleCommand
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0001135D File Offset: 0x0000F55D
		public string Command
		{
			get
			{
				return "listvotes";
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000341 RID: 833 RVA: 0x00011364 File Offset: 0x0000F564
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-listvotes-desc");
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00011370 File Offset: 0x0000F570
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-listvotes-help");
			}
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0001137C File Offset: 0x0000F57C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			foreach (IVoteHandle vote in IoCManager.Resolve<IVoteManager>().ActiveVotes)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				defaultInterpolatedStringHandler.AppendFormatted<int>(vote.Id);
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				defaultInterpolatedStringHandler.AppendFormatted(vote.InitiatorText);
				defaultInterpolatedStringHandler.AppendLiteral(": ");
				defaultInterpolatedStringHandler.AppendFormatted(vote.Title);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}
}
