using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Maps
{
	// Token: 0x020003D6 RID: 982
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class GridDraggingCommand : IConsoleCommand
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600142C RID: 5164 RVA: 0x000689F6 File Offset: 0x00066BF6
		public string Command
		{
			get
			{
				return "griddrag";
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x0600142D RID: 5165 RVA: 0x000689FD File Offset: 0x00066BFD
		public string Description
		{
			get
			{
				return "Allows someone with permissions to drag grids around.";
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x0600142E RID: 5166 RVA: 0x00068A04 File Offset: 0x00066C04
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x00068A18 File Offset: 0x00066C18
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (shell.Player == null)
			{
				shell.WriteError("shell-server-cannot");
				return;
			}
			GridDraggingSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GridDraggingSystem>();
			entitySystem.Toggle(shell.Player);
			if (entitySystem.IsEnabled(shell.Player))
			{
				shell.WriteLine("Grid dragging toggled on");
				return;
			}
			shell.WriteLine("Grid dragging toggled off");
		}
	}
}
