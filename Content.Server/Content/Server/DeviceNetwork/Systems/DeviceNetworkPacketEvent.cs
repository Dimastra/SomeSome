using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000586 RID: 1414
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeviceNetworkPacketEvent : EntityEventArgs
	{
		// Token: 0x06001DA2 RID: 7586 RVA: 0x0009DC7B File Offset: 0x0009BE7B
		public DeviceNetworkPacketEvent(int netId, [Nullable(2)] string address, uint frequency, string senderAddress, EntityUid sender, NetworkPayload data)
		{
			this.NetId = netId;
			this.Address = address;
			this.Frequency = frequency;
			this.SenderAddress = senderAddress;
			this.Sender = sender;
			this.Data = data;
		}

		// Token: 0x040012FE RID: 4862
		public int NetId;

		// Token: 0x040012FF RID: 4863
		public readonly uint Frequency;

		// Token: 0x04001300 RID: 4864
		[Nullable(2)]
		public string Address;

		// Token: 0x04001301 RID: 4865
		public readonly string SenderAddress;

		// Token: 0x04001302 RID: 4866
		public EntityUid Sender;

		// Token: 0x04001303 RID: 4867
		public readonly NetworkPayload Data;
	}
}
