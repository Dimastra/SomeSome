using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Managers;
using Content.Server.Voting.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Voting
{
	// Token: 0x020000C0 RID: 192
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class CreateCustomCommand : IConsoleCommand
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000334 RID: 820 RVA: 0x00010E95 File Offset: 0x0000F095
		public string Command
		{
			get
			{
				return "customvote";
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000335 RID: 821 RVA: 0x00010E9C File Offset: 0x0000F09C
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-customvote-desc");
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000336 RID: 822 RVA: 0x00010EA8 File Offset: 0x0000F0A8
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-customvote-help");
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00010EB4 File Offset: 0x0000F0B4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 3 || args.Length > 10)
			{
				shell.WriteError(Loc.GetString("shell-need-between-arguments", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("lower", 3),
					new ValueTuple<string, object>("upper", 10)
				}));
				return;
			}
			string title = args[0];
			IVoteManager mgr = IoCManager.Resolve<IVoteManager>();
			VoteOptions options = new VoteOptions
			{
				Title = title,
				Duration = TimeSpan.FromSeconds(30.0)
			};
			for (int i = 1; i < args.Length; i++)
			{
				options.Options.Add(new ValueTuple<string, object>(args[i], i));
			}
			options.SetInitiatorOrServer((IPlayerSession)shell.Player);
			if (shell.Player != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(29, 3);
				logStringHandler.AppendFormatted<ICommonSession>(shell.Player, "shell.Player");
				logStringHandler.AppendLiteral(" initiated a custom vote: ");
				logStringHandler.AppendFormatted(options.Title);
				logStringHandler.AppendLiteral(" - ");
				logStringHandler.AppendFormatted(string.Join("; ", from x in options.Options
				select x.Item1));
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Vote;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(28, 2);
				logStringHandler.AppendLiteral("Initiated a custom vote: ");
				logStringHandler.AppendFormatted(options.Title);
				logStringHandler.AppendLiteral(" - ");
				logStringHandler.AppendFormatted(string.Join("; ", from x in options.Options
				select x.Item1));
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			IVoteHandle vote = mgr.CreateVote(options);
			Func<object, string> <>9__3;
			vote.OnFinished += delegate(IVoteHandle _, VoteFinishedEventArgs eventArgs)
			{
				IChatManager chatMgr = IoCManager.Resolve<IChatManager>();
				LogStringHandler logStringHandler2;
				if (eventArgs.Winner == null)
				{
					string separator = ", ";
					ImmutableArray<object> winners = eventArgs.Winners;
					Func<object, string> selector;
					if ((selector = <>9__3) == null)
					{
						selector = (<>9__3 = ((object c) => args[(int)c]));
					}
					string ties = string.Join(separator, winners.Select(selector));
					ISharedAdminLogManager adminLogger3 = this._adminLogger;
					LogType type3 = LogType.Vote;
					LogImpact impact3 = LogImpact.Medium;
					logStringHandler2 = new LogStringHandler(30, 2);
					logStringHandler2.AppendLiteral("Custom vote ");
					logStringHandler2.AppendFormatted(options.Title);
					logStringHandler2.AppendLiteral(" finished as tie: ");
					logStringHandler2.AppendFormatted(ties);
					adminLogger3.Add(type3, impact3, ref logStringHandler2);
					chatMgr.DispatchServerAnnouncement(Loc.GetString("cmd-customvote-on-finished-tie", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("ties", ties)
					}), null);
					this.DisplayVoteResult(vote, chatMgr, args);
					return;
				}
				ISharedAdminLogManager adminLogger4 = this._adminLogger;
				LogType type4 = LogType.Vote;
				LogImpact impact4 = LogImpact.Medium;
				logStringHandler2 = new LogStringHandler(23, 2);
				logStringHandler2.AppendLiteral("Custom vote ");
				logStringHandler2.AppendFormatted(options.Title);
				logStringHandler2.AppendLiteral(" finished: ");
				logStringHandler2.AppendFormatted(args[(int)eventArgs.Winner]);
				adminLogger4.Add(type4, impact4, ref logStringHandler2);
				chatMgr.DispatchServerAnnouncement(Loc.GetString("cmd-customvote-on-finished-win", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("winner", args[(int)eventArgs.Winner])
				}), null);
				this.DisplayVoteResult(vote, chatMgr, args);
			};
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00011108 File Offset: 0x0000F308
		private void DisplayVoteResult(IVoteHandle vote, IChatManager chatMgr, string[] args)
		{
			chatMgr.DispatchServerAnnouncement(Loc.GetString("cmd-customvote-on-finished-votes", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("title", vote.Title)
			}), null);
			for (int x = 1; x < args.Length; x++)
			{
				string option = args[x];
				int votes = vote.VotesPerOption[x];
				chatMgr.DispatchServerAnnouncement(Loc.GetString("cmd-customvote-option-votes", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("option", option),
					new ValueTuple<string, object>("votes", votes)
				}), null);
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000111B8 File Offset: 0x0000F3B8
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHint(Loc.GetString("cmd-customvote-arg-title"));
			}
			if (args.Length > 10)
			{
				return CompletionResult.Empty;
			}
			int i = args.Length - 1;
			return CompletionResult.FromHint(Loc.GetString("cmd-customvote-arg-option-n", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("n", i)
			}));
		}

		// Token: 0x04000219 RID: 537
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400021A RID: 538
		private const int MaxArgCount = 10;
	}
}
