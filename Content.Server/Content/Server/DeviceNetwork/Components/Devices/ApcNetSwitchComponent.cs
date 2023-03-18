using System;
using Content.Server.DeviceNetwork.Systems.Devices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.DeviceNetwork.Components.Devices
{
	// Token: 0x02000592 RID: 1426
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ApcNetSwitchSystem)
	})]
	public sealed class ApcNetSwitchComponent : Component
	{
		// Token: 0x0400131B RID: 4891
		[ViewVariables]
		public bool State;
	}
}
