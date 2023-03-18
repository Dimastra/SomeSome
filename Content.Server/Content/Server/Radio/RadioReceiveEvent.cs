using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;

namespace Content.Server.Radio
{
	// Token: 0x02000258 RID: 600
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadioReceiveEvent : EntityEventArgs
	{
		// Token: 0x06000BE1 RID: 3041 RVA: 0x0003EAA0 File Offset: 0x0003CCA0
		public RadioReceiveEvent(string message, EntityUid source, RadioChannelPrototype channel, MsgChatMessage chatMsg, EntityUid? radioSource)
		{
			this.Message = message;
			this.Source = source;
			this.Channel = channel;
			this.ChatMsg = chatMsg;
			this.RadioSource = radioSource;
		}

		// Token: 0x0400076D RID: 1901
		public readonly string Message;

		// Token: 0x0400076E RID: 1902
		public readonly EntityUid Source;

		// Token: 0x0400076F RID: 1903
		public readonly RadioChannelPrototype Channel;

		// Token: 0x04000770 RID: 1904
		public readonly MsgChatMessage ChatMsg;

		// Token: 0x04000771 RID: 1905
		public readonly EntityUid? RadioSource;
	}
}
