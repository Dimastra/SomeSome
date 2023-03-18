using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen
{
	// Token: 0x02000391 RID: 913
	[NetSerializable]
	[Serializable]
	public enum GrinderProgram : byte
	{
		// Token: 0x04000A74 RID: 2676
		Grind,
		// Token: 0x04000A75 RID: 2677
		Juice
	}
}
