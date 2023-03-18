using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Borgs
{
	// Token: 0x020000A4 RID: 164
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class AddLawCommand : IConsoleCommand
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000D9CF File Offset: 0x0000BBCF
		public string Command
		{
			get
			{
				return "lawadd";
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000D9D6 File Offset: 0x0000BBD6
		public string Description
		{
			get
			{
				return Loc.GetString("command-lawadd-description");
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0000D9E2 File Offset: 0x0000BBE2
		public string Help
		{
			get
			{
				return Loc.GetString("command-lawadd-help");
			}
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000D9F0 File Offset: 0x0000BBF0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			AddLawCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<AddLawCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
