using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Whitelist
{
	// Token: 0x02000081 RID: 129
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class KickNonWhitelistedCommand : IConsoleCommand
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000A677 File Offset: 0x00008877
		public string Command
		{
			get
			{
				return "kicknonwhitelisted";
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x0000A67E File Offset: 0x0000887E
		public string Description
		{
			get
			{
				return Loc.GetString("command-kicknonwhitelisted-description");
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000A68A File Offset: 0x0000888A
		public string Help
		{
			get
			{
				return Loc.GetString("command-kicknonwhitelisted-help");
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000A698 File Offset: 0x00008898
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			KickNonWhitelistedCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<KickNonWhitelistedCommand.<Execute>d__6>(ref <Execute>d__);
		}
	}
}
