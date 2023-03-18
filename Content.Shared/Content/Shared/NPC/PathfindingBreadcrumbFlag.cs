using System;

namespace Content.Shared.NPC
{
	// Token: 0x020002CA RID: 714
	[Flags]
	public enum PathfindingBreadcrumbFlag : ushort
	{
		// Token: 0x04000803 RID: 2051
		None = 0,
		// Token: 0x04000804 RID: 2052
		Invalid = 1,
		// Token: 0x04000805 RID: 2053
		Space = 2,
		// Token: 0x04000806 RID: 2054
		Door = 4,
		// Token: 0x04000807 RID: 2055
		Access = 8
	}
}
