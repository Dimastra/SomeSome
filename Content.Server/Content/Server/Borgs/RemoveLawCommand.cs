using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Borgs
{
	// Token: 0x020000A5 RID: 165
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class RemoveLawCommand : IConsoleCommand
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000DA37 File Offset: 0x0000BC37
		public string Command
		{
			get
			{
				return "lawrm";
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000296 RID: 662 RVA: 0x0000DA3E File Offset: 0x0000BC3E
		public string Description
		{
			get
			{
				return Loc.GetString("command-lawrm-description");
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000DA4A File Offset: 0x0000BC4A
		public string Help
		{
			get
			{
				return Loc.GetString("command-lawrm-help");
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000DA58 File Offset: 0x0000BC58
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RemoveLawCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<RemoveLawCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
