using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000352 RID: 850
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SignalPortsState : BoundUserInterfaceState
	{
		// Token: 0x060009F1 RID: 2545 RVA: 0x00020746 File Offset: 0x0001E946
		public SignalPortsState(string transmitterName, List<string> transmitterPorts, string receiverName, List<string> receiverPorts, [Nullable(new byte[]
		{
			1,
			0
		})] List<ValueTuple<int, int>> links)
		{
			this.TransmitterName = transmitterName;
			this.TransmitterPorts = transmitterPorts;
			this.ReceiverName = receiverName;
			this.ReceiverPorts = receiverPorts;
			this.Links = links;
		}

		// Token: 0x040009AF RID: 2479
		public readonly string TransmitterName;

		// Token: 0x040009B0 RID: 2480
		public readonly List<string> TransmitterPorts;

		// Token: 0x040009B1 RID: 2481
		public readonly string ReceiverName;

		// Token: 0x040009B2 RID: 2482
		public readonly List<string> ReceiverPorts;

		// Token: 0x040009B3 RID: 2483
		[Nullable(new byte[]
		{
			1,
			0
		})]
		public readonly List<ValueTuple<int, int>> Links;
	}
}
