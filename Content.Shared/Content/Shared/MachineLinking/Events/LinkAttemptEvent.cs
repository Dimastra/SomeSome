using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.MachineLinking.Events
{
	// Token: 0x02000356 RID: 854
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LinkAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x060009F5 RID: 2549 RVA: 0x00020799 File Offset: 0x0001E999
		public LinkAttemptEvent(EntityUid? user, EntityUid transmitter, string transmitterPort, EntityUid receiver, string receiverPort)
		{
			this.User = user;
			this.Transmitter = transmitter;
			this.TransmitterPort = transmitterPort;
			this.Receiver = receiver;
			this.ReceiverPort = receiverPort;
		}

		// Token: 0x040009B6 RID: 2486
		public readonly EntityUid Transmitter;

		// Token: 0x040009B7 RID: 2487
		public readonly EntityUid Receiver;

		// Token: 0x040009B8 RID: 2488
		public readonly EntityUid? User;

		// Token: 0x040009B9 RID: 2489
		public readonly string TransmitterPort;

		// Token: 0x040009BA RID: 2490
		public readonly string ReceiverPort;
	}
}
