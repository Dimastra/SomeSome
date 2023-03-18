using System;
using System.Runtime.CompilerServices;
using Content.Client.Computer;
using Content.Shared.Power;
using Robust.Client.GameObjects;

namespace Content.Client.Power
{
	// Token: 0x0200019A RID: 410
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public sealed class PowerMonitoringConsoleBoundUserInterface : ComputerBoundUserInterface<PowerMonitoringWindow, PowerMonitoringConsoleBoundInterfaceState>
	{
		// Token: 0x06000AE6 RID: 2790 RVA: 0x0003F62D File Offset: 0x0003D82D
		public PowerMonitoringConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}
	}
}
