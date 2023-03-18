using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Medical.SuitSensor
{
	// Token: 0x0200030B RID: 779
	[NetSerializable]
	[Serializable]
	public enum SuitSensorMode : byte
	{
		// Token: 0x040008E1 RID: 2273
		SensorOff,
		// Token: 0x040008E2 RID: 2274
		SensorBinary,
		// Token: 0x040008E3 RID: 2275
		SensorVitals,
		// Token: 0x040008E4 RID: 2276
		SensorCords
	}
}
