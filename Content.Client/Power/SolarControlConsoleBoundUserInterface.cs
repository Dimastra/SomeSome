using System;
using System.Runtime.CompilerServices;
using Content.Client.Computer;
using Content.Shared.Solar;
using Robust.Client.GameObjects;

namespace Content.Client.Power
{
	// Token: 0x0200019E RID: 414
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public sealed class SolarControlConsoleBoundUserInterface : ComputerBoundUserInterface<SolarControlWindow, SolarControlConsoleBoundInterfaceState>
	{
		// Token: 0x06000AFC RID: 2812 RVA: 0x0004014C File Offset: 0x0003E34C
		public SolarControlConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}
	}
}
