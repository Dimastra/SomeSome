using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D4 RID: 1748
	[NetSerializable]
	[Serializable]
	public enum AirAlarmWireStatus
	{
		// Token: 0x0400155B RID: 5467
		Power,
		// Token: 0x0400155C RID: 5468
		Access,
		// Token: 0x0400155D RID: 5469
		Panic,
		// Token: 0x0400155E RID: 5470
		DeviceSync
	}
}
