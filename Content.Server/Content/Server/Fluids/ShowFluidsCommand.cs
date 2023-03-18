using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Fluids.EntitySystems;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Fluids
{
	// Token: 0x020004EA RID: 1258
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class ShowFluidsCommand : IConsoleCommand
	{
		// Token: 0x170003DB RID: 987
		// (get) Token: 0x060019E5 RID: 6629 RVA: 0x00087C14 File Offset: 0x00085E14
		public string Command
		{
			get
			{
				return "showfluids";
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x060019E6 RID: 6630 RVA: 0x00087C1B File Offset: 0x00085E1B
		public string Description
		{
			get
			{
				return "Toggles seeing puddle debug overlay.";
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x060019E7 RID: 6631 RVA: 0x00087C22 File Offset: 0x00085E22
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x00087C34 File Offset: 0x00085E34
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("You must be a player to use this command.");
				return;
			}
			shell.WriteLine(this._entitySystem.GetEntitySystem<PuddleDebugDebugOverlaySystem>().ToggleObserver(player) ? "Enabled the puddle debug overlay." : "Disabled the puddle debug overlay.");
		}

		// Token: 0x04001049 RID: 4169
		[Dependency]
		private readonly IEntitySystemManager _entitySystem;
	}
}
