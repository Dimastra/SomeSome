using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C4 RID: 1732
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EntitySpokeEvent : EntityEventArgs
	{
		// Token: 0x06002425 RID: 9253 RVA: 0x000BC9E1 File Offset: 0x000BABE1
		public EntitySpokeEvent(EntityUid source, string message, string originalMessage, [Nullable(2)] RadioChannelPrototype channel, [Nullable(2)] string obfuscatedMessage)
		{
			this.Source = source;
			this.Message = message;
			this.OriginalMessage = originalMessage;
			this.Channel = channel;
			this.ObfuscatedMessage = obfuscatedMessage;
		}

		// Token: 0x04001659 RID: 5721
		public readonly EntityUid Source;

		// Token: 0x0400165A RID: 5722
		public readonly string Message;

		// Token: 0x0400165B RID: 5723
		public readonly string OriginalMessage;

		// Token: 0x0400165C RID: 5724
		[Nullable(2)]
		public readonly string ObfuscatedMessage;

		// Token: 0x0400165D RID: 5725
		[Nullable(2)]
		public RadioChannelPrototype Channel;
	}
}
