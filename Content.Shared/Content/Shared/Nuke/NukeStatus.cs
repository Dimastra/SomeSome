using System;

namespace Content.Shared.Nuke
{
	// Token: 0x020002BE RID: 702
	public enum NukeStatus : byte
	{
		// Token: 0x040007DF RID: 2015
		AWAIT_DISK,
		// Token: 0x040007E0 RID: 2016
		AWAIT_CODE,
		// Token: 0x040007E1 RID: 2017
		AWAIT_ARM,
		// Token: 0x040007E2 RID: 2018
		ARMED,
		// Token: 0x040007E3 RID: 2019
		COOLDOWN
	}
}
