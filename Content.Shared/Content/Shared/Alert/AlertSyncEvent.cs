using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Alert
{
	// Token: 0x02000722 RID: 1826
	public sealed class AlertSyncEvent : EntityEventArgs
	{
		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x0600162F RID: 5679 RVA: 0x00048AEE File Offset: 0x00046CEE
		public EntityUid Euid { get; }

		// Token: 0x06001630 RID: 5680 RVA: 0x00048AF6 File Offset: 0x00046CF6
		public AlertSyncEvent(EntityUid euid)
		{
			this.Euid = euid;
		}
	}
}
