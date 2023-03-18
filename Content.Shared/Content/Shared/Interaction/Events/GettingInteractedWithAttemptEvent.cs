using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D8 RID: 984
	public sealed class GettingInteractedWithAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000B8D RID: 2957 RVA: 0x00026329 File Offset: 0x00024529
		public GettingInteractedWithAttemptEvent(EntityUid uid, EntityUid? target)
		{
			this.Uid = uid;
			this.Target = target;
		}

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000B8E RID: 2958 RVA: 0x0002633F File Offset: 0x0002453F
		public EntityUid Uid { get; }

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000B8F RID: 2959 RVA: 0x00026347 File Offset: 0x00024547
		public EntityUid? Target { get; }
	}
}
