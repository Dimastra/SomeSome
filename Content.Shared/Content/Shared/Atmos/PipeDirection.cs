using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos
{
	// Token: 0x02000695 RID: 1685
	[Flags]
	[NetSerializable]
	[Serializable]
	public enum PipeDirection
	{
		// Token: 0x04001480 RID: 5248
		None = 0,
		// Token: 0x04001481 RID: 5249
		North = 1,
		// Token: 0x04001482 RID: 5250
		South = 2,
		// Token: 0x04001483 RID: 5251
		West = 4,
		// Token: 0x04001484 RID: 5252
		East = 8,
		// Token: 0x04001485 RID: 5253
		Longitudinal = 3,
		// Token: 0x04001486 RID: 5254
		Lateral = 12,
		// Token: 0x04001487 RID: 5255
		NWBend = 5,
		// Token: 0x04001488 RID: 5256
		NEBend = 9,
		// Token: 0x04001489 RID: 5257
		SWBend = 6,
		// Token: 0x0400148A RID: 5258
		SEBend = 10,
		// Token: 0x0400148B RID: 5259
		TNorth = 13,
		// Token: 0x0400148C RID: 5260
		TSouth = 14,
		// Token: 0x0400148D RID: 5261
		TWest = 7,
		// Token: 0x0400148E RID: 5262
		TEast = 11,
		// Token: 0x0400148F RID: 5263
		Fourway = 15,
		// Token: 0x04001490 RID: 5264
		All = -1
	}
}
