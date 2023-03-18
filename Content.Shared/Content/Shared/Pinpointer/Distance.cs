using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer
{
	// Token: 0x02000275 RID: 629
	[NetSerializable]
	[Serializable]
	public enum Distance : byte
	{
		// Token: 0x04000718 RID: 1816
		Unknown,
		// Token: 0x04000719 RID: 1817
		Reached,
		// Token: 0x0400071A RID: 1818
		Close,
		// Token: 0x0400071B RID: 1819
		Medium,
		// Token: 0x0400071C RID: 1820
		Far
	}
}
