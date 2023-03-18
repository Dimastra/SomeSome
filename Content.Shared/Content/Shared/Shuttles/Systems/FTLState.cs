using System;

namespace Content.Shared.Shuttles.Systems
{
	// Token: 0x020001BC RID: 444
	[Flags]
	public enum FTLState : byte
	{
		// Token: 0x04000514 RID: 1300
		Invalid = 0,
		// Token: 0x04000515 RID: 1301
		Available = 1,
		// Token: 0x04000516 RID: 1302
		Starting = 2,
		// Token: 0x04000517 RID: 1303
		Travelling = 4,
		// Token: 0x04000518 RID: 1304
		Arriving = 8,
		// Token: 0x04000519 RID: 1305
		Cooldown = 16
	}
}
