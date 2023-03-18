using System;
using System.Runtime.CompilerServices;
using Content.Shared.EntityHealthBar;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003AF RID: 943
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ToggleHealthBarsCommand : IConsoleCommand
	{
		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x00086C28 File Offset: 0x00084E28
		public string Command
		{
			get
			{
				return "togglehealthbars";
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06001770 RID: 6000 RVA: 0x00086C2F File Offset: 0x00084E2F
		public string Description
		{
			get
			{
				return "Toggles a health bar above mobs.";
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06001771 RID: 6001 RVA: 0x00086C36 File Offset: 0x00084E36
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x00086C48 File Offset: 0x00084E48
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null)
			{
				shell.WriteLine("You aren't a player.");
				return;
			}
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				shell.WriteLine("You do not have an attached entity.");
				return;
			}
			ShowHealthBarsComponent showHealthBarsComponent;
			if (!this._entityManager.TryGetComponent<ShowHealthBarsComponent>(entityUid, ref showHealthBarsComponent))
			{
				this._entityManager.AddComponent<ShowHealthBarsComponent>(entityUid.Value);
				shell.WriteLine("Enabled health overlay.");
				return;
			}
			this._entityManager.RemoveComponent<ShowHealthBarsComponent>(entityUid.Value);
			shell.WriteLine("Disabled health overlay.");
		}

		// Token: 0x04000BFF RID: 3071
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000C00 RID: 3072
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
