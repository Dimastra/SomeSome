using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C2 RID: 1730
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TransformSpeechEvent : EntityEventArgs
	{
		// Token: 0x06002423 RID: 9251 RVA: 0x000BC9B5 File Offset: 0x000BABB5
		public TransformSpeechEvent(EntityUid sender, string message)
		{
			this.Sender = sender;
			this.Message = message;
		}

		// Token: 0x04001655 RID: 5717
		public EntityUid Sender;

		// Token: 0x04001656 RID: 5718
		public string Message;
	}
}
