using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000510 RID: 1296
	[NetSerializable]
	[Serializable]
	public sealed class NetworkConfiguratorComponentState : ComponentState
	{
		// Token: 0x06000FC5 RID: 4037 RVA: 0x00032EC7 File Offset: 0x000310C7
		public NetworkConfiguratorComponentState(EntityUid? activeDeviceList)
		{
			this.ActiveDeviceList = activeDeviceList;
		}

		// Token: 0x04000EF5 RID: 3829
		public readonly EntityUid? ActiveDeviceList;
	}
}
