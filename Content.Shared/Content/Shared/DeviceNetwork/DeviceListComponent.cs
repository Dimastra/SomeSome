using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x0200050D RID: 1293
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedDeviceListSystem)
	})]
	public sealed class DeviceListComponent : Component
	{
		// Token: 0x04000EEB RID: 3819
		[Nullable(1)]
		[DataField("devices", false, 1, false, false, null)]
		public HashSet<EntityUid> Devices = new HashSet<EntityUid>();

		// Token: 0x04000EEC RID: 3820
		[ViewVariables]
		[DataField("deviceLimit", false, 1, false, false, null)]
		public int DeviceLimit = 32;

		// Token: 0x04000EED RID: 3821
		[ViewVariables]
		[DataField("isAllowList", false, 1, false, false, null)]
		public bool IsAllowList = true;

		// Token: 0x04000EEE RID: 3822
		[ViewVariables]
		[DataField("handleIncoming", false, 1, false, false, null)]
		public bool HandleIncomingPackets;
	}
}
