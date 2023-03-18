using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004ED RID: 1261
	[NetSerializable]
	[Serializable]
	public enum DoorState
	{
		// Token: 0x04000E72 RID: 3698
		Closed,
		// Token: 0x04000E73 RID: 3699
		Closing,
		// Token: 0x04000E74 RID: 3700
		Open,
		// Token: 0x04000E75 RID: 3701
		Opening,
		// Token: 0x04000E76 RID: 3702
		Welded,
		// Token: 0x04000E77 RID: 3703
		Denying,
		// Token: 0x04000E78 RID: 3704
		Emagging
	}
}
