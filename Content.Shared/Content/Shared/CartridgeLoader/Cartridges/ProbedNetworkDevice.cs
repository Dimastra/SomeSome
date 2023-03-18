using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CartridgeLoader.Cartridges
{
	// Token: 0x02000623 RID: 1571
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[DataRecord]
	[Serializable]
	public sealed class ProbedNetworkDevice
	{
		// Token: 0x0600130C RID: 4876 RVA: 0x0003F9C8 File Offset: 0x0003DBC8
		public ProbedNetworkDevice(string name, string address, string frequency, string netId)
		{
			this.Name = name;
			this.Address = address;
			this.Frequency = frequency;
			this.NetId = netId;
		}

		// Token: 0x040012E9 RID: 4841
		public readonly string Name;

		// Token: 0x040012EA RID: 4842
		public readonly string Address;

		// Token: 0x040012EB RID: 4843
		public readonly string Frequency;

		// Token: 0x040012EC RID: 4844
		public readonly string NetId;
	}
}
