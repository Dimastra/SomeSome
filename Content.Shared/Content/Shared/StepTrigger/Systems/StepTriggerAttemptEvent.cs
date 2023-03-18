using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Systems
{
	// Token: 0x0200014B RID: 331
	[ByRefEvent]
	public struct StepTriggerAttemptEvent
	{
		// Token: 0x040003D3 RID: 979
		public EntityUid Source;

		// Token: 0x040003D4 RID: 980
		public EntityUid Tripper;

		// Token: 0x040003D5 RID: 981
		public bool Continue;

		// Token: 0x040003D6 RID: 982
		public bool Cancelled;
	}
}
