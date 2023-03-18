using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085C RID: 2140
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class RoleBanListCommand : IConsoleCommand
	{
		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06002ECC RID: 11980 RVA: 0x000F2EBC File Offset: 0x000F10BC
		public string Command
		{
			get
			{
				return "rolebanlist";
			}
		}

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06002ECD RID: 11981 RVA: 0x000F2EC3 File Offset: 0x000F10C3
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-rolebanlist-desc");
			}
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06002ECE RID: 11982 RVA: 0x000F2ECF File Offset: 0x000F10CF
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-rolebanlist-help");
			}
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x000F2EDC File Offset: 0x000F10DC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RoleBanListCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<RoleBanListCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x000F2F24 File Offset: 0x000F1124
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			int num = args.Length;
			CompletionResult result;
			if (num != 1)
			{
				if (num != 2)
				{
					result = CompletionResult.Empty;
				}
				else
				{
					result = CompletionResult.FromHintOptions(CompletionHelper.Booleans, Loc.GetString("cmd-rolebanlist-hint-2"));
				}
			}
			else
			{
				result = CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, null), Loc.GetString("cmd-rolebanlist-hint-1"));
			}
			return result;
		}
	}
}
