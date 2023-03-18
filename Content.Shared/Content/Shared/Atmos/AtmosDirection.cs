using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos
{
	// Token: 0x0200068C RID: 1676
	[Flags]
	[FlagsFor(typeof(AtmosDirectionFlags))]
	[Serializable]
	public enum AtmosDirection
	{
		// Token: 0x04001413 RID: 5139
		Invalid = 0,
		// Token: 0x04001414 RID: 5140
		North = 1,
		// Token: 0x04001415 RID: 5141
		South = 2,
		// Token: 0x04001416 RID: 5142
		East = 4,
		// Token: 0x04001417 RID: 5143
		West = 8,
		// Token: 0x04001418 RID: 5144
		NorthEast = 5,
		// Token: 0x04001419 RID: 5145
		SouthEast = 6,
		// Token: 0x0400141A RID: 5146
		NorthWest = 9,
		// Token: 0x0400141B RID: 5147
		SouthWest = 10,
		// Token: 0x0400141C RID: 5148
		All = 15
	}
}
