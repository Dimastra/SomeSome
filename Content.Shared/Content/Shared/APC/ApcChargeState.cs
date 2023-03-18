using System;
using Robust.Shared.Serialization;

namespace Content.Shared.APC
{
	// Token: 0x020006F2 RID: 1778
	[NetSerializable]
	[Serializable]
	public enum ApcChargeState
	{
		// Token: 0x040015A6 RID: 5542
		Lack,
		// Token: 0x040015A7 RID: 5543
		Charging,
		// Token: 0x040015A8 RID: 5544
		Full,
		// Token: 0x040015A9 RID: 5545
		Emag
	}
}
