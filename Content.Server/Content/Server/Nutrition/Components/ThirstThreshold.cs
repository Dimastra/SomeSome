using System;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031F RID: 799
	[Flags]
	public enum ThirstThreshold : byte
	{
		// Token: 0x040009AC RID: 2476
		Dead = 0,
		// Token: 0x040009AD RID: 2477
		Parched = 1,
		// Token: 0x040009AE RID: 2478
		Thirsty = 2,
		// Token: 0x040009AF RID: 2479
		Okay = 4,
		// Token: 0x040009B0 RID: 2480
		OverHydrated = 8
	}
}
