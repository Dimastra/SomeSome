using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Systems
{
	// Token: 0x0200014C RID: 332
	[ByRefEvent]
	public struct StepTriggeredEvent
	{
		// Token: 0x040003D7 RID: 983
		public EntityUid Source;

		// Token: 0x040003D8 RID: 984
		public EntityUid Tripper;
	}
}
