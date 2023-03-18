using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085D RID: 2141
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class RoleUnbanCommand : IConsoleCommand
	{
		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x06002ED2 RID: 11986 RVA: 0x000F2F7E File Offset: 0x000F117E
		public string Command
		{
			get
			{
				return "roleunban";
			}
		}

		// Token: 0x170007A0 RID: 1952
		// (get) Token: 0x06002ED3 RID: 11987 RVA: 0x000F2F85 File Offset: 0x000F1185
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-roleunban-desc");
			}
		}

		// Token: 0x170007A1 RID: 1953
		// (get) Token: 0x06002ED4 RID: 11988 RVA: 0x000F2F91 File Offset: 0x000F1191
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-roleunban-help");
			}
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x000F2FA0 File Offset: 0x000F11A0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RoleUnbanCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<RoleUnbanCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x000F2FE8 File Offset: 0x000F11E8
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			CompletionResult result;
			if (args.Length == 1)
			{
				result = CompletionResult.FromHint(Loc.GetString("cmd-roleunban-hint-1"));
			}
			else
			{
				result = CompletionResult.Empty;
			}
			return result;
		}
	}
}
