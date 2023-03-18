using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.Roles;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200083C RID: 2108
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class DepartmentBanCommand : IConsoleCommand
	{
		// Token: 0x1700073E RID: 1854
		// (get) Token: 0x06002E22 RID: 11810 RVA: 0x000F0FDC File Offset: 0x000EF1DC
		public string Command
		{
			get
			{
				return "departmentban";
			}
		}

		// Token: 0x1700073F RID: 1855
		// (get) Token: 0x06002E23 RID: 11811 RVA: 0x000F0FE3 File Offset: 0x000EF1E3
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-departmentban-desc");
			}
		}

		// Token: 0x17000740 RID: 1856
		// (get) Token: 0x06002E24 RID: 11812 RVA: 0x000F0FEF File Offset: 0x000EF1EF
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-departmentban-help");
			}
		}

		// Token: 0x06002E25 RID: 11813 RVA: 0x000F0FFC File Offset: 0x000EF1FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			DepartmentBanCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<DepartmentBanCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002E26 RID: 11814 RVA: 0x000F1044 File Offset: 0x000EF244
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
				result = CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<DepartmentPrototype>(true, null), Loc.GetString("cmd-roleban-hint-2"));
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
