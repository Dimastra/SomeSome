using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Info
{
	// Token: 0x02000450 RID: 1104
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ShowRulesCommand : IConsoleCommand
	{
		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001641 RID: 5697 RVA: 0x000757B7 File Offset: 0x000739B7
		public string Command
		{
			get
			{
				return "showrules";
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001642 RID: 5698 RVA: 0x000757BE File Offset: 0x000739BE
		public string Description
		{
			get
			{
				return "Opens the rules popup for the specified player.";
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001643 RID: 5699 RVA: 0x000757C5 File Offset: 0x000739C5
		public string Help
		{
			get
			{
				return "showrules <username> [seconds]";
			}
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x000757CC File Offset: 0x000739CC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ShowRulesCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<ShowRulesCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
