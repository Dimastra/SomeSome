using System;
using System.Runtime.CompilerServices;
using Content.Client.NPC.HTN;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NPC
{
	// Token: 0x02000212 RID: 530
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowHTNCommand : IConsoleCommand
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000DDE RID: 3550 RVA: 0x000546CC File Offset: 0x000528CC
		public string Command
		{
			get
			{
				return "showhtn";
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06000DDF RID: 3551 RVA: 0x000546D3 File Offset: 0x000528D3
		public string Description
		{
			get
			{
				return "Shows the current status for HTN NPCs";
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x000546DA File Offset: 0x000528DA
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x000546EB File Offset: 0x000528EB
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			HTNSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<HTNSystem>();
			entitySystem.EnableOverlay = !entitySystem.EnableOverlay;
		}
	}
}
