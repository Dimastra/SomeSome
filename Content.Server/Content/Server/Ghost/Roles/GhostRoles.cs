using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Ghost.Roles
{
	// Token: 0x02000496 RID: 1174
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class GhostRoles : IConsoleCommand
	{
		// Token: 0x17000333 RID: 819
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x0007BD2C File Offset: 0x00079F2C
		public string Command
		{
			get
			{
				return "ghostroles";
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060017A0 RID: 6048 RVA: 0x0007BD33 File Offset: 0x00079F33
		public string Description
		{
			get
			{
				return "Opens the ghost role request window.";
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x0007BD3A File Offset: 0x00079F3A
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x0007BD4B File Offset: 0x00079F4B
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (shell.Player != null)
			{
				EntitySystem.Get<GhostRoleSystem>().OpenEui((IPlayerSession)shell.Player);
				return;
			}
			shell.WriteLine("You can only open the ghost roles UI on a client.");
		}
	}
}
