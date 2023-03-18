using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Whitelist
{
	// Token: 0x0200007F RID: 127
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class AddWhitelistCommand : IConsoleCommand
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000A5A9 File Offset: 0x000087A9
		public string Command
		{
			get
			{
				return "whitelistadd";
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000A5B0 File Offset: 0x000087B0
		public string Description
		{
			get
			{
				return Loc.GetString("command-whitelistadd-description");
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000A5BC File Offset: 0x000087BC
		public string Help
		{
			get
			{
				return Loc.GetString("command-whitelistadd-help");
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000A5C8 File Offset: 0x000087C8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			AddWhitelistCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<AddWhitelistCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
