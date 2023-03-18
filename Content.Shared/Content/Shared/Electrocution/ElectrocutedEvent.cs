using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Electrocution
{
	// Token: 0x020004CD RID: 1229
	public sealed class ElectrocutedEvent : EntityEventArgs
	{
		// Token: 0x06000EDD RID: 3805 RVA: 0x0002FD77 File Offset: 0x0002DF77
		public ElectrocutedEvent(EntityUid targetUid, EntityUid? sourceUid, float siemensCoefficient)
		{
			this.TargetUid = targetUid;
			this.SourceUid = sourceUid;
			this.SiemensCoefficient = siemensCoefficient;
		}

		// Token: 0x04000DF5 RID: 3573
		public readonly EntityUid TargetUid;

		// Token: 0x04000DF6 RID: 3574
		public readonly EntityUid? SourceUid;

		// Token: 0x04000DF7 RID: 3575
		public readonly float SiemensCoefficient;
	}
}
