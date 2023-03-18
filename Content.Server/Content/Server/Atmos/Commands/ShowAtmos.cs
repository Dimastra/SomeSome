using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B9 RID: 1977
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class ShowAtmos : IConsoleCommand
	{
		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06002ADC RID: 10972 RVA: 0x000E0B14 File Offset: 0x000DED14
		public string Command
		{
			get
			{
				return "showatmos";
			}
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x06002ADD RID: 10973 RVA: 0x000E0B1B File Offset: 0x000DED1B
		public string Description
		{
			get
			{
				return "Toggles seeing atmos debug overlay.";
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06002ADE RID: 10974 RVA: 0x000E0B22 File Offset: 0x000DED22
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x000E0B34 File Offset: 0x000DED34
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You must be a player to use this command.");
				return;
			}
			shell.WriteLine(EntitySystem.Get<AtmosDebugOverlaySystem>().ToggleObserver(player) ? "Enabled the atmospherics debug overlay." : "Disabled the atmospherics debug overlay.");
		}
	}
}
