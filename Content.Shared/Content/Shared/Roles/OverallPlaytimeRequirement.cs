using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Roles
{
	// Token: 0x020001E8 RID: 488
	public sealed class OverallPlaytimeRequirement : JobRequirement
	{
		// Token: 0x0400058F RID: 1423
		[DataField("time", false, 1, false, false, null)]
		public TimeSpan Time;

		// Token: 0x04000590 RID: 1424
		[DataField("inverted", false, 1, false, false, null)]
		public bool Inverted;
	}
}
