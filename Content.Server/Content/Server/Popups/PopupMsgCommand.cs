using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Popups;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Popups
{
	// Token: 0x020002C1 RID: 705
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class PopupMsgCommand : IConsoleCommand
	{
		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x000480A0 File Offset: 0x000462A0
		public string Command
		{
			get
			{
				return "srvpopupmsg";
			}
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000E3A RID: 3642 RVA: 0x000480A7 File Offset: 0x000462A7
		public string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000E3B RID: 3643 RVA: 0x000480AE File Offset: 0x000462AE
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x000480B8 File Offset: 0x000462B8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			EntityUid source = EntityUid.Parse(args[0]);
			EntityUid viewer = EntityUid.Parse(args[1]);
			string msg = args[2];
			source.PopupMessage(viewer, msg);
		}
	}
}
