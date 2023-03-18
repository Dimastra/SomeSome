using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Interaction.Events
{
	// Token: 0x020003D1 RID: 977
	public sealed class AttackAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0002618C File Offset: 0x0002438C
		public EntityUid Uid { get; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x00026194 File Offset: 0x00024394
		public EntityUid? Target { get; }

		// Token: 0x06000B78 RID: 2936 RVA: 0x0002619C File Offset: 0x0002439C
		public AttackAttemptEvent(EntityUid uid, EntityUid? target = null)
		{
			this.Uid = uid;
			this.Target = target;
		}
	}
}
