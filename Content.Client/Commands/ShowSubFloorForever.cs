using System;
using System.Runtime.CompilerServices;
using Content.Client.SubFloor;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A6 RID: 934
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ShowSubFloorForever : IConsoleCommand
	{
		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001740 RID: 5952 RVA: 0x00086607 File Offset: 0x00084807
		public string Command
		{
			get
			{
				return "showsubfloorforever";
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06001741 RID: 5953 RVA: 0x0008660E File Offset: 0x0008480E
		public string Description
		{
			get
			{
				return "Makes entities below the floor always visible until the client is restarted.";
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06001742 RID: 5954 RVA: 0x00086615 File Offset: 0x00084815
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x00086628 File Offset: 0x00084828
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			EntitySystem.Get<SubFloorHideSystem>().ShowAll = true;
			foreach (ValueTuple<SubFloorHideComponent, SpriteComponent> valueTuple in IoCManager.Resolve<IEntityManager>().EntityQuery<SubFloorHideComponent, SpriteComponent>(true))
			{
				valueTuple.Item2.DrawDepth = 9;
			}
		}
	}
}
