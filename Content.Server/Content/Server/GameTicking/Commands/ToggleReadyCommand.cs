using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004DF RID: 1247
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class ToggleReadyCommand : IConsoleCommand
	{
		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x060019B9 RID: 6585 RVA: 0x00086A5A File Offset: 0x00084C5A
		public string Command
		{
			get
			{
				return "toggleready";
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x060019BA RID: 6586 RVA: 0x00086A61 File Offset: 0x00084C61
		public string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x060019BB RID: 6587 RVA: 0x00086A68 File Offset: 0x00084C68
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x00086A70 File Offset: 0x00084C70
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			if (player == null)
			{
				return;
			}
			EntitySystem.Get<GameTicker>().ToggleReady(player, bool.Parse(args[0]));
		}
	}
}
