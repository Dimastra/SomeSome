using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000713 RID: 1811
	[RegisterComponent]
	public sealed class AnomalyPulsingComponent : Component
	{
		// Token: 0x0400162B RID: 5675
		[DataField("endTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan EndTime = TimeSpan.MaxValue;

		// Token: 0x0400162C RID: 5676
		[ViewVariables]
		public TimeSpan PulseDuration = TimeSpan.FromSeconds(5.0);
	}
}
