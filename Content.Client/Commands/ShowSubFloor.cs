using System;
using System.Runtime.CompilerServices;
using Content.Client.SubFloor;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A5 RID: 933
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ShowSubFloor : IConsoleCommand
	{
		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x0600173B RID: 5947 RVA: 0x000865CD File Offset: 0x000847CD
		public string Command
		{
			get
			{
				return "showsubfloor";
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x0600173C RID: 5948 RVA: 0x000865D4 File Offset: 0x000847D4
		public string Description
		{
			get
			{
				return "Makes entities below the floor always visible.";
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x0600173D RID: 5949 RVA: 0x000865DB File Offset: 0x000847DB
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x000865ED File Offset: 0x000847ED
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			SubFloorHideSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SubFloorHideSystem>();
			entitySystem.ShowAll = !entitySystem.ShowAll;
		}
	}
}
