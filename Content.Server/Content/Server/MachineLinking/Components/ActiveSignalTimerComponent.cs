using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.MachineLinking.Components
{
	// Token: 0x020003FB RID: 1019
	[RegisterComponent]
	public sealed class ActiveSignalTimerComponent : Component
	{
		// Token: 0x04000CD9 RID: 3289
		[DataField("triggerTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan TriggerTime;
	}
}
