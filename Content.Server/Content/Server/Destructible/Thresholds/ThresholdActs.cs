using System;
using Robust.Shared.Serialization;

namespace Content.Server.Destructible.Thresholds
{
	// Token: 0x0200059B RID: 1435
	[Flags]
	[FlagsFor(typeof(ActsFlags))]
	[Serializable]
	public enum ThresholdActs
	{
		// Token: 0x04001334 RID: 4916
		None = 0,
		// Token: 0x04001335 RID: 4917
		Breakage = 1,
		// Token: 0x04001336 RID: 4918
		Destruction = 2
	}
}
