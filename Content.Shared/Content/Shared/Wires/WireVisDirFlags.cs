using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x0200002C RID: 44
	[Flags]
	[NetSerializable]
	[Serializable]
	public enum WireVisDirFlags : byte
	{
		// Token: 0x04000081 RID: 129
		None = 0,
		// Token: 0x04000082 RID: 130
		North = 1,
		// Token: 0x04000083 RID: 131
		South = 2,
		// Token: 0x04000084 RID: 132
		East = 4,
		// Token: 0x04000085 RID: 133
		West = 8
	}
}
