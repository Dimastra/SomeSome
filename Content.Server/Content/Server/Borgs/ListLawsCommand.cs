using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Borgs
{
	// Token: 0x020000A2 RID: 162
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Logs)]
	public sealed class ListLawsCommand : IConsoleCommand
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000D901 File Offset: 0x0000BB01
		public string Command
		{
			get
			{
				return "lslaws";
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000287 RID: 647 RVA: 0x0000D908 File Offset: 0x0000BB08
		public string Description
		{
			get
			{
				return Loc.GetString("command-lslaws-description");
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000D914 File Offset: 0x0000BB14
		public string Help
		{
			get
			{
				return Loc.GetString("command-lslaws-help");
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000D920 File Offset: 0x0000BB20
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ListLawsCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<ListLawsCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
