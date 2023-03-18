using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085B RID: 2139
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class RoleBanCommand : IConsoleCommand
	{
		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06002EC6 RID: 11974 RVA: 0x000F2D15 File Offset: 0x000F0F15
		public string Command
		{
			get
			{
				return "roleban";
			}
		}

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x000F2D1C File Offset: 0x000F0F1C
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-roleban-desc");
			}
		}

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06002EC8 RID: 11976 RVA: 0x000F2D28 File Offset: 0x000F0F28
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-roleban-help");
			}
		}

		// Token: 0x06002EC9 RID: 11977 RVA: 0x000F2D34 File Offset: 0x000F0F34
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RoleBanCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<RoleBanCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002ECA RID: 11978 RVA: 0x000F2D7C File Offset: 0x000F0F7C
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			CompletionOption[] durOpts = new CompletionOption[]
			{
				new CompletionOption("0", Loc.GetString("cmd-roleban-hint-duration-1"), 0),
				new CompletionOption("1440", Loc.GetString("cmd-roleban-hint-duration-2"), 0),
				new CompletionOption("4320", Loc.GetString("cmd-roleban-hint-duration-3"), 0),
				new CompletionOption("10080", Loc.GetString("cmd-roleban-hint-duration-4"), 0),
				new CompletionOption("20160", Loc.GetString("cmd-roleban-hint-duration-5"), 0),
				new CompletionOption("43800", Loc.GetString("cmd-roleban-hint-duration-6"), 0)
			};
			CompletionResult result;
			switch (args.Length)
			{
			case 1:
				result = CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, null), Loc.GetString("cmd-roleban-hint-1"));
				break;
			case 2:
				result = CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<JobPrototype>(true, null), Loc.GetString("cmd-roleban-hint-2"));
				break;
			case 3:
				result = CompletionResult.FromHint(Loc.GetString("cmd-roleban-hint-3"));
				break;
			case 4:
				result = CompletionResult.FromHintOptions(durOpts, Loc.GetString("cmd-roleban-hint-4"));
				break;
			default:
				result = CompletionResult.Empty;
				break;
			}
			return result;
		}
	}
}
