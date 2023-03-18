using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Systems;
using Robust.Shared.GameObjects;

namespace Content.Server.CartridgeLoader
{
	// Token: 0x020006DA RID: 1754
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CartridgeDeviceNetPacketEvent : EntityEventArgs
	{
		// Token: 0x060024AC RID: 9388 RVA: 0x000BF030 File Offset: 0x000BD230
		public CartridgeDeviceNetPacketEvent(EntityUid loader, DeviceNetworkPacketEvent packetEvent)
		{
			this.Loader = loader;
			this.PacketEvent = packetEvent;
		}

		// Token: 0x04001684 RID: 5764
		public readonly EntityUid Loader;

		// Token: 0x04001685 RID: 5765
		public readonly DeviceNetworkPacketEvent PacketEvent;
	}
}
