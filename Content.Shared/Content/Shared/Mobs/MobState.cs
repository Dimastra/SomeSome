using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs
{
	// Token: 0x020002FB RID: 763
	[NetSerializable]
	[Serializable]
	public enum MobState : byte
	{
		// Token: 0x040008B1 RID: 2225
		Invalid,
		// Token: 0x040008B2 RID: 2226
		Alive,
		// Token: 0x040008B3 RID: 2227
		Critical,
		// Token: 0x040008B4 RID: 2228
		Dead
	}
}
