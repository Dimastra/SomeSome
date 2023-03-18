using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.EUI;
using Content.Server.NPC.UI;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.NPC.Commands
{
	// Token: 0x02000375 RID: 885
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class NPCCommand : IConsoleCommand
	{
		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06001217 RID: 4631 RVA: 0x0005EC45 File Offset: 0x0005CE45
		public string Command
		{
			get
			{
				return "npc";
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x0005EC4C File Offset: 0x0005CE4C
		public string Description
		{
			get
			{
				return "Opens the debug window for NPCs";
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06001219 RID: 4633 RVA: 0x0005EC53 File Offset: 0x0005CE53
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0005EC64 File Offset: 0x0005CE64
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession playerSession = shell.Player as IPlayerSession;
			if (playerSession == null)
			{
				return;
			}
			IoCManager.Resolve<EuiManager>().OpenEui(new NPCEui(), playerSession);
		}
	}
}
