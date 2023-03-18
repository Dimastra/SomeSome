using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x02000714 RID: 1812
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class AnomalySupercriticalComponent : Component
	{
		// Token: 0x0400162D RID: 5677
		[DataField("endTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan EndTime = TimeSpan.MaxValue;

		// Token: 0x0400162E RID: 5678
		[ViewVariables]
		public TimeSpan SupercriticalDuration = TimeSpan.FromSeconds(10.0);

		// Token: 0x0400162F RID: 5679
		[DataField("maxScaleAmount", false, 1, false, false, null)]
		public float MaxScaleAmount = 3f;
	}
}
