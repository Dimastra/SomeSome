using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components
{
	// Token: 0x020002B5 RID: 693
	[NetSerializable]
	[Serializable]
	public enum HungerThreshold : byte
	{
		// Token: 0x040007D5 RID: 2005
		Overfed = 8,
		// Token: 0x040007D6 RID: 2006
		Okay = 4,
		// Token: 0x040007D7 RID: 2007
		Peckish = 2,
		// Token: 0x040007D8 RID: 2008
		Starving = 1,
		// Token: 0x040007D9 RID: 2009
		Dead = 0
	}
}
