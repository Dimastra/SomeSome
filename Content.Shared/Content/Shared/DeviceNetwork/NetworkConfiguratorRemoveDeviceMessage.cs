using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000514 RID: 1300
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class NetworkConfiguratorRemoveDeviceMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000FC8 RID: 4040 RVA: 0x00032EE6 File Offset: 0x000310E6
		public NetworkConfiguratorRemoveDeviceMessage(string address)
		{
			this.Address = address;
		}

		// Token: 0x04000F03 RID: 3843
		public readonly string Address;
	}
}
