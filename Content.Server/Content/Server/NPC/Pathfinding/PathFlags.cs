using System;

namespace Content.Server.NPC.Pathfinding
{
	// Token: 0x0200033B RID: 827
	[Flags]
	public enum PathFlags : byte
	{
		// Token: 0x04000A5B RID: 2651
		None = 0,
		// Token: 0x04000A5C RID: 2652
		Access = 1,
		// Token: 0x04000A5D RID: 2653
		Prying = 2,
		// Token: 0x04000A5E RID: 2654
		Smashing = 4,
		// Token: 0x04000A5F RID: 2655
		Interact = 8
	}
}
