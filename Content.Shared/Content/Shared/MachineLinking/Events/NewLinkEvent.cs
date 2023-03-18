using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.MachineLinking.Events
{
	// Token: 0x02000357 RID: 855
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NewLinkEvent : EntityEventArgs
	{
		// Token: 0x060009F6 RID: 2550 RVA: 0x000207C6 File Offset: 0x0001E9C6
		public NewLinkEvent(EntityUid? user, EntityUid transmitter, string transmitterPort, EntityUid receiver, string receiverPort)
		{
			this.User = user;
			this.Transmitter = transmitter;
			this.TransmitterPort = transmitterPort;
			this.Receiver = receiver;
			this.ReceiverPort = receiverPort;
		}

		// Token: 0x040009BB RID: 2491
		public readonly EntityUid Transmitter;

		// Token: 0x040009BC RID: 2492
		public readonly EntityUid Receiver;

		// Token: 0x040009BD RID: 2493
		public readonly EntityUid? User;

		// Token: 0x040009BE RID: 2494
		public readonly string TransmitterPort;

		// Token: 0x040009BF RID: 2495
		public readonly string ReceiverPort;
	}
}
