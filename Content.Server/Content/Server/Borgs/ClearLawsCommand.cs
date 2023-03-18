using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Borgs
{
	// Token: 0x020000A3 RID: 163
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class ClearLawsCommand : IConsoleCommand
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600028B RID: 651 RVA: 0x0000D967 File Offset: 0x0000BB67
		public string Command
		{
			get
			{
				return "lawclear";
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000D96E File Offset: 0x0000BB6E
		public string Description
		{
			get
			{
				return Loc.GetString("command-lawclear-description");
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600028D RID: 653 RVA: 0x0000D97A File Offset: 0x0000BB7A
		public string Help
		{
			get
			{
				return Loc.GetString("command-lawclear-help");
			}
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000D988 File Offset: 0x0000BB88
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ClearLawsCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<ClearLawsCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
