using System;
using Robust.Shared.Serialization;

namespace Content.Shared.PAI
{
	// Token: 0x020002A9 RID: 681
	[NetSerializable]
	[Serializable]
	public enum PAIStatus : byte
	{
		// Token: 0x040007B5 RID: 1973
		Off,
		// Token: 0x040007B6 RID: 1974
		Searching,
		// Token: 0x040007B7 RID: 1975
		On
	}
}
