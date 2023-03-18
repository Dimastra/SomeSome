using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004EE RID: 1262
	[NetSerializable]
	[Serializable]
	public enum DoorVisuals
	{
		// Token: 0x04000E7A RID: 3706
		State,
		// Token: 0x04000E7B RID: 3707
		Powered,
		// Token: 0x04000E7C RID: 3708
		BoltLights,
		// Token: 0x04000E7D RID: 3709
		EmergencyLights,
		// Token: 0x04000E7E RID: 3710
		ClosedLights,
		// Token: 0x04000E7F RID: 3711
		BaseRSI
	}
}
