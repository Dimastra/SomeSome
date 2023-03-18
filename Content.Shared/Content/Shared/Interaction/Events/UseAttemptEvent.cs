using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003DC RID: 988
	public sealed class UseAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000B9B RID: 2971 RVA: 0x000263EB File Offset: 0x000245EB
		public UseAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000B9C RID: 2972 RVA: 0x000263FA File Offset: 0x000245FA
		public EntityUid Uid { get; }
	}
}
