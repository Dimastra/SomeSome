using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000848 RID: 2120
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class OSay : LocalizedCommands
	{
		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06002E61 RID: 11873 RVA: 0x000F1D3A File Offset: 0x000EFF3A
		public override string Command
		{
			get
			{
				return "osay";
			}
		}

		// Token: 0x06002E62 RID: 11874 RVA: 0x000F1D44 File Offset: 0x000EFF44
		public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHint(Loc.GetString("osay-command-arg-uid"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHintOptions(Enum.GetNames(typeof(InGameICChatType)), Loc.GetString("osay-command-arg-type"));
			}
			if (args.Length > 2)
			{
				return CompletionResult.FromHint(Loc.GetString("osay-command-arg-message"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x000F1DA8 File Offset: 0x000EFFA8
		public override void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 3)
			{
				shell.WriteLine(Loc.GetString("osay-command-error-args"));
				return;
			}
			InGameICChatType chatType = (InGameICChatType)Enum.Parse(typeof(InGameICChatType), args[1]);
			EntityUid source;
			if (!EntityUid.TryParse(args[0], ref source) || !this._entityManager.EntityExists(source))
			{
				shell.WriteLine(Loc.GetString("osay-command-error-euid", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[0])
				}));
				return;
			}
			string message = string.Join(" ", args.Skip(2)).Trim();
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			this._entityManager.System<ChatSystem>().TrySendInGameICMessage(source, message, chatType, false, false, null, null, null, true, false);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Action;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(14, 4);
			logStringHandler.AppendFormatted((shell.Player != null) ? shell.Player.Name : "An administrator");
			logStringHandler.AppendLiteral(" forced ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this._entityManager.ToPrettyString(source), "_entityManager.ToPrettyString(source)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted(args[1]);
			logStringHandler.AppendLiteral(": ");
			logStringHandler.AppendFormatted(message);
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x04001C51 RID: 7249
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001C52 RID: 7250
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
