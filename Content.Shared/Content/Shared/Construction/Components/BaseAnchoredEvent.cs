using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x02000588 RID: 1416
	public abstract class BaseAnchoredEvent : EntityEventArgs
	{
		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06001165 RID: 4453 RVA: 0x00039119 File Offset: 0x00037319
		public EntityUid User { get; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06001166 RID: 4454 RVA: 0x00039121 File Offset: 0x00037321
		public EntityUid Tool { get; }

		// Token: 0x06001167 RID: 4455 RVA: 0x00039129 File Offset: 0x00037329
		protected BaseAnchoredEvent(EntityUid user, EntityUid tool)
		{
			this.User = user;
			this.Tool = tool;
		}
	}
}
