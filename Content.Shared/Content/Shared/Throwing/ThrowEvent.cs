using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D3 RID: 211
	public abstract class ThrowEvent : HandledEntityEventArgs
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000B249 File Offset: 0x00009449
		public EntityUid? User { get; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600024E RID: 590 RVA: 0x0000B251 File Offset: 0x00009451
		public EntityUid Thrown { get; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600024F RID: 591 RVA: 0x0000B259 File Offset: 0x00009459
		public EntityUid Target { get; }

		// Token: 0x06000250 RID: 592 RVA: 0x0000B261 File Offset: 0x00009461
		public ThrowEvent(EntityUid? user, EntityUid thrown, EntityUid target)
		{
			this.User = user;
			this.Thrown = thrown;
			this.Target = target;
		}
	}
}
