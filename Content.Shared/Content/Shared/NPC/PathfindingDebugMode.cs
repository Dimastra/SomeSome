using System;

namespace Content.Shared.NPC
{
	// Token: 0x020002CB RID: 715
	[Flags]
	public enum PathfindingDebugMode : ushort
	{
		// Token: 0x04000809 RID: 2057
		None = 0,
		// Token: 0x0400080A RID: 2058
		Breadcrumbs = 1,
		// Token: 0x0400080B RID: 2059
		Chunks = 2,
		// Token: 0x0400080C RID: 2060
		Crumb = 4,
		// Token: 0x0400080D RID: 2061
		Polys = 8,
		// Token: 0x0400080E RID: 2062
		PolyNeighbors = 16,
		// Token: 0x0400080F RID: 2063
		Poly = 32,
		// Token: 0x04000810 RID: 2064
		Routes = 64,
		// Token: 0x04000811 RID: 2065
		RouteCosts = 128,
		// Token: 0x04000812 RID: 2066
		Steering = 256
	}
}
