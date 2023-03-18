using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200084B RID: 2123
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class PardonCommand : IConsoleCommand
	{
		// Token: 0x17000769 RID: 1897
		// (get) Token: 0x06002E6F RID: 11887 RVA: 0x000F2081 File Offset: 0x000F0281
		public string Command
		{
			get
			{
				return "pardon";
			}
		}

		// Token: 0x1700076A RID: 1898
		// (get) Token: 0x06002E70 RID: 11888 RVA: 0x000F2088 File Offset: 0x000F0288
		public string Description
		{
			get
			{
				return "Pardons somebody's ban";
			}
		}

		// Token: 0x1700076B RID: 1899
		// (get) Token: 0x06002E71 RID: 11889 RVA: 0x000F208F File Offset: 0x000F028F
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <ban id>";
			}
		}

		// Token: 0x06002E72 RID: 11890 RVA: 0x000F20A8 File Offset: 0x000F02A8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PardonCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<PardonCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
