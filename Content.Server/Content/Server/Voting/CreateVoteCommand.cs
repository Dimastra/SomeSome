using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Voting;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Voting
{
	// Token: 0x020000BF RID: 191
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class CreateVoteCommand : IConsoleCommand
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00010D65 File Offset: 0x0000EF65
		public string Command
		{
			get
			{
				return "createvote";
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600032F RID: 815 RVA: 0x00010D6C File Offset: 0x0000EF6C
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-createvote-desc");
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00010D78 File Offset: 0x0000EF78
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-createvote-help");
			}
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00010D84 File Offset: 0x0000EF84
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
				return;
			}
			StandardVoteType type;
			if (!Enum.TryParse<StandardVoteType>(args[0], true, out type))
			{
				shell.WriteError(Loc.GetString("cmd-createvote-invalid-vote-type"));
				return;
			}
			IVoteManager mgr = IoCManager.Resolve<IVoteManager>();
			if (shell.Player != null && !mgr.CanCallVote((IPlayerSession)shell.Player, new StandardVoteType?(type)))
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type2 = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(22, 2);
				logStringHandler.AppendFormatted<ICommonSession>(shell.Player, "shell.Player");
				logStringHandler.AppendLiteral(" failed to start ");
				logStringHandler.AppendFormatted(type.ToString());
				logStringHandler.AppendLiteral(" vote");
				adminLogger.Add(type2, impact, ref logStringHandler);
				shell.WriteError(Loc.GetString("cmd-createvote-cannot-call-vote-now"));
				return;
			}
			mgr.CreateStandardVote((IPlayerSession)shell.Player, type);
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00010E6B File Offset: 0x0000F06B
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(Enum.GetNames<StandardVoteType>(), Loc.GetString("cmd-createvote-arg-vote-type"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04000218 RID: 536
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
