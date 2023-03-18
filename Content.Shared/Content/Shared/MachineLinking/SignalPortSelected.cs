using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000353 RID: 851
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class SignalPortSelected : BoundUserInterfaceMessage
	{
		// Token: 0x060009F2 RID: 2546 RVA: 0x00020773 File Offset: 0x0001E973
		public SignalPortSelected(string transmitterPort, string receiverPort)
		{
			this.TransmitterPort = transmitterPort;
			this.ReceiverPort = receiverPort;
		}

		// Token: 0x040009B4 RID: 2484
		public readonly string TransmitterPort;

		// Token: 0x040009B5 RID: 2485
		public readonly string ReceiverPort;
	}
}
