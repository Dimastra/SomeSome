using System;

namespace Content.Server.Atmos.Reactions
{
	// Token: 0x0200073F RID: 1855
	[Flags]
	public enum ReactionResult : byte
	{
		// Token: 0x04001837 RID: 6199
		NoReaction = 0,
		// Token: 0x04001838 RID: 6200
		Reacting = 1,
		// Token: 0x04001839 RID: 6201
		StopReactions = 2
	}
}
