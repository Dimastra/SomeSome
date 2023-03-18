using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Whitelist
{
	// Token: 0x02000080 RID: 128
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class RemoveWhitelistCommand : IConsoleCommand
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000A60F File Offset: 0x0000880F
		public string Command
		{
			get
			{
				return "whitelistremove";
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000A616 File Offset: 0x00008816
		public string Description
		{
			get
			{
				return Loc.GetString("command-whitelistremove-description");
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000A622 File Offset: 0x00008822
		public string Help
		{
			get
			{
				return Loc.GetString("command-whitelistremove-help");
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000A630 File Offset: 0x00008830
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			RemoveWhitelistCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<RemoveWhitelistCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
