using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;

namespace Content.Server.Radio
{
	// Token: 0x02000259 RID: 601
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadioReceiveAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000BE2 RID: 3042 RVA: 0x0003EACD File Offset: 0x0003CCCD
		public RadioReceiveAttemptEvent(string message, EntityUid source, RadioChannelPrototype channel, EntityUid? radioSource)
		{
			this.Message = message;
			this.Source = source;
			this.Channel = channel;
			this.RadioSource = radioSource;
		}

		// Token: 0x04000772 RID: 1906
		public readonly string Message;

		// Token: 0x04000773 RID: 1907
		public readonly EntityUid Source;

		// Token: 0x04000774 RID: 1908
		public readonly RadioChannelPrototype Channel;

		// Token: 0x04000775 RID: 1909
		public readonly EntityUid? RadioSource;
	}
}
