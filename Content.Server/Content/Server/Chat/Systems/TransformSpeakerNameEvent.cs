using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C0 RID: 1728
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TransformSpeakerNameEvent : EntityEventArgs
	{
		// Token: 0x0600241D RID: 9245 RVA: 0x000BC967 File Offset: 0x000BAB67
		public TransformSpeakerNameEvent(EntityUid sender, string name)
		{
			this.Sender = sender;
			this.Name = name;
		}

		// Token: 0x04001651 RID: 5713
		public EntityUid Sender;

		// Token: 0x04001652 RID: 5714
		public string Name;
	}
}
