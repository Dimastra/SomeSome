using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D7 RID: 983
	public sealed class InteractionAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000B8A RID: 2954 RVA: 0x00026303 File Offset: 0x00024503
		public InteractionAttemptEvent(EntityUid uid, EntityUid? target)
		{
			this.Uid = uid;
			this.Target = target;
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000B8B RID: 2955 RVA: 0x00026319 File Offset: 0x00024519
		public EntityUid Uid { get; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000B8C RID: 2956 RVA: 0x00026321 File Offset: 0x00024521
		public EntityUid? Target { get; }
	}
}
