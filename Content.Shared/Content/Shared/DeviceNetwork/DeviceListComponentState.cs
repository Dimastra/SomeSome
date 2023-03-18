using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x0200050E RID: 1294
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DeviceListComponentState : ComponentState
	{
		// Token: 0x06000FC3 RID: 4035 RVA: 0x00032E70 File Offset: 0x00031070
		public DeviceListComponentState(HashSet<EntityUid> devices, bool isAllowList, bool handleIncomingPackets)
		{
			this.Devices = devices;
			this.IsAllowList = isAllowList;
			this.HandleIncomingPackets = handleIncomingPackets;
		}

		// Token: 0x04000EEF RID: 3823
		public readonly HashSet<EntityUid> Devices;

		// Token: 0x04000EF0 RID: 3824
		public readonly bool IsAllowList;

		// Token: 0x04000EF1 RID: 3825
		public readonly bool HandleIncomingPackets;
	}
}
