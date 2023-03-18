using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Electrocution
{
	// Token: 0x020004CC RID: 1228
	public sealed class ElectrocutionAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06000EDB RID: 3803 RVA: 0x0002FD3F File Offset: 0x0002DF3F
		public SlotFlags TargetSlots { get; }

		// Token: 0x06000EDC RID: 3804 RVA: 0x0002FD47 File Offset: 0x0002DF47
		public ElectrocutionAttemptEvent(EntityUid targetUid, EntityUid? sourceUid, float siemensCoefficient, SlotFlags targetSlots)
		{
			this.TargetUid = targetUid;
			this.TargetSlots = targetSlots;
			this.SourceUid = sourceUid;
			this.SiemensCoefficient = siemensCoefficient;
		}

		// Token: 0x04000DF2 RID: 3570
		public readonly EntityUid TargetUid;

		// Token: 0x04000DF3 RID: 3571
		public readonly EntityUid? SourceUid;

		// Token: 0x04000DF4 RID: 3572
		public float SiemensCoefficient = 1f;
	}
}
