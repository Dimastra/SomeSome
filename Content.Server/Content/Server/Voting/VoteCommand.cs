using System;
using System.Runtime.CompilerServices;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Voting
{
	// Token: 0x020000C1 RID: 193
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class VoteCommand : IConsoleCommand
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600033B RID: 827 RVA: 0x00011222 File Offset: 0x0000F422
		public string Command
		{
			get
			{
				return "vote";
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00011229 File Offset: 0x0000F429
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-vote-desc");
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600033D RID: 829 RVA: 0x00011235 File Offset: 0x0000F435
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-vote-help");
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00011244 File Offset: 0x0000F444
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (shell.Player == null)
			{
				shell.WriteError(Loc.GetString("cmd-vote-on-execute-error-must-be-player"));
				return;
			}
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("properAmount", 2),
					new ValueTuple<string, object>("currentAmount", args.Length)
				}));
				return;
			}
			int voteId;
			if (!int.TryParse(args[0], out voteId))
			{
				shell.WriteError(Loc.GetString("cmd-vote-on-execute-error-invalid-vote-id"));
				return;
			}
			int voteOption;
			if (!int.TryParse(args[1], out voteOption))
			{
				shell.WriteError(Loc.GetString("cmd-vote-on-execute-error-invalid-vote-options"));
				return;
			}
			IVoteHandle vote;
			if (!IoCManager.Resolve<IVoteManager>().TryGetVote(voteId, out vote))
			{
				shell.WriteError(Loc.GetString("cmd-vote-on-execute-error-invalid-vote"));
				return;
			}
			int? optionN;
			if (voteOption == -1)
			{
				optionN = null;
			}
			else
			{
				if (!vote.IsValidOption(voteOption))
				{
					shell.WriteError(Loc.GetString("cmd-vote-on-execute-error-invalid-option"));
					return;
				}
				optionN = new int?(voteOption);
			}
			vote.CastVote((IPlayerSession)shell.Player, optionN);
		}
	}
}
