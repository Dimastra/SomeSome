using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Morgue
{
	// Token: 0x020002F8 RID: 760
	[NetSerializable]
	[Serializable]
	public enum MorgueContents : byte
	{
		// Token: 0x040008A9 RID: 2217
		Empty,
		// Token: 0x040008AA RID: 2218
		HasMob,
		// Token: 0x040008AB RID: 2219
		HasSoul,
		// Token: 0x040008AC RID: 2220
		HasContents
	}
}
