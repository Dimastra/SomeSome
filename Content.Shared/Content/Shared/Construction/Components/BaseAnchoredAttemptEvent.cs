using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000585 RID: 1413
	public abstract class BaseAnchoredAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x1700037C RID: 892
		// (get) Token: 0x0600115E RID: 4446 RVA: 0x000390CE File Offset: 0x000372CE
		public EntityUid User { get; }

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x0600115F RID: 4447 RVA: 0x000390D6 File Offset: 0x000372D6
		public EntityUid Tool { get; }

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06001160 RID: 4448 RVA: 0x000390DE File Offset: 0x000372DE
		// (set) Token: 0x06001161 RID: 4449 RVA: 0x000390E6 File Offset: 0x000372E6
		public float Delay { get; set; }

		// Token: 0x06001162 RID: 4450 RVA: 0x000390EF File Offset: 0x000372EF
		protected BaseAnchoredAttemptEvent(EntityUid user, EntityUid tool)
		{
			this.User = user;
			this.Tool = tool;
		}
	}
}
