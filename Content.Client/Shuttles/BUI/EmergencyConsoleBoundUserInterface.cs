using System;
using System.Runtime.CompilerServices;
using Content.Client.Computer;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Robust.Client.GameObjects;

namespace Content.Client.Shuttles.BUI
{
	// Token: 0x02000158 RID: 344
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public sealed class EmergencyConsoleBoundUserInterface : ComputerBoundUserInterface<EmergencyConsoleWindow, EmergencyConsoleBoundUserInterfaceState>
	{
		// Token: 0x06000912 RID: 2322 RVA: 0x0003592F File Offset: 0x00033B2F
		public EmergencyConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}
	}
}
