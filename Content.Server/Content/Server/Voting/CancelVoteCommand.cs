using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Voting
{
	// Token: 0x020000C3 RID: 195
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class CancelVoteCommand : IConsoleCommand
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000345 RID: 837 RVA: 0x00011430 File Offset: 0x0000F630
		public string Command
		{
			get
			{
				return "cancelvote";
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00011437 File Offset: 0x0000F637
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-cancelvote-desc");
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000347 RID: 839 RVA: 0x00011443 File Offset: 0x0000F643
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-cancelvote-help");
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00011450 File Offset: 0x0000F650
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IVoteManager mgr = IoCManager.Resolve<IVoteManager>();
			if (args.Length < 1)
			{
				shell.WriteError(Loc.GetString("cmd-cancelvote-error-missing-vote-id"));
				return;
			}
			int id;
			IVoteHandle vote;
			if (!int.TryParse(args[0], out id) || !mgr.TryGetVote(id, out vote))
			{
				shell.WriteError(Loc.GetString("cmd-cancelvote-error-invalid-vote-id"));
				return;
			}
			if (shell.Player != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(16, 2);
				logStringHandler.AppendFormatted<ICommonSession>(shell.Player, "shell.Player");
				logStringHandler.AppendLiteral(" canceled vote: ");
				logStringHandler.AppendFormatted(vote.Title);
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Vote;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(15, 1);
				logStringHandler.AppendLiteral("Canceled vote: ");
				logStringHandler.AppendFormatted(vote.Title);
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			vote.Cancel();
		}

		// Token: 0x06000349 RID: 841 RVA: 0x0001152C File Offset: 0x0000F72C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			IVoteManager mgr = IoCManager.Resolve<IVoteManager>();
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(from v in mgr.ActiveVotes
				orderby v.Id
				select new CompletionOption(v.Id.ToString(), v.Title, 0), Loc.GetString("cmd-cancelvote-arg-id"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x0400021B RID: 539
		[Dependency]
		private readonly IAdminLogManager _adminLogger;
	}
}
