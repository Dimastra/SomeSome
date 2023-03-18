using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000715 RID: 1813
	[NetSerializable]
	[Serializable]
	public sealed class AnomalySupercriticalComponentState : ComponentState
	{
		// Token: 0x04001630 RID: 5680
		public TimeSpan EndTime;

		// Token: 0x04001631 RID: 5681
		public TimeSpan Duration;
	}
}
