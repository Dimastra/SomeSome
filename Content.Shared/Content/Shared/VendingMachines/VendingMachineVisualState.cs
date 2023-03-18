using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines
{
	// Token: 0x02000098 RID: 152
	[NetSerializable]
	[Serializable]
	public enum VendingMachineVisualState
	{
		// Token: 0x04000223 RID: 547
		Normal,
		// Token: 0x04000224 RID: 548
		Off,
		// Token: 0x04000225 RID: 549
		Broken,
		// Token: 0x04000226 RID: 550
		Eject,
		// Token: 0x04000227 RID: 551
		Deny
	}
}
