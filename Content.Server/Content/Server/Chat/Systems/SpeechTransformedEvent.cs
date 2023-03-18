using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C3 RID: 1731
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpeechTransformedEvent : EntityEventArgs
	{
		// Token: 0x06002424 RID: 9252 RVA: 0x000BC9CB File Offset: 0x000BABCB
		public SpeechTransformedEvent(EntityUid sender, string message)
		{
			this.Sender = sender;
			this.Message = message;
		}

		// Token: 0x04001657 RID: 5719
		public EntityUid Sender;

		// Token: 0x04001658 RID: 5720
		public string Message;
	}
}
