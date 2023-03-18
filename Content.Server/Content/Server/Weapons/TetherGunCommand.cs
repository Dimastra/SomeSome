using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Weapons
{
	// Token: 0x020000AC RID: 172
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class TetherGunCommand : IConsoleCommand
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000DFDD File Offset: 0x0000C1DD
		public string Command
		{
			get
			{
				return "tethergun";
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002B0 RID: 688 RVA: 0x0000DFE4 File Offset: 0x0000C1E4
		public string Description
		{
			get
			{
				return "Allows you to drag mobs around with your mouse.";
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000DFEB File Offset: 0x0000C1EB
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000DFFC File Offset: 0x0000C1FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			TetherGunSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<TetherGunSystem>();
			entitySystem.Toggle(shell.Player);
			if (entitySystem.IsEnabled(shell.Player))
			{
				shell.WriteLine("Tether gun toggled on");
				return;
			}
			shell.WriteLine("Tether gun toggled off");
		}
	}
}
