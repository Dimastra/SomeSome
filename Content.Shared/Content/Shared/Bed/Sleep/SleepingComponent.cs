using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Bed.Sleep
{
	// Token: 0x02000674 RID: 1652
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class SleepingComponent : Component
	{
		// Token: 0x040013E3 RID: 5091
		[DataField("wakeThreshold", false, 1, false, false, null)]
		public FixedPoint2 WakeThreshold = FixedPoint2.New(2);

		// Token: 0x040013E4 RID: 5092
		[DataField("cooldown", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan Cooldown = TimeSpan.FromSeconds(1.0);

		// Token: 0x040013E5 RID: 5093
		[DataField("cooldownEnd", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan CoolDownEnd;
	}
}
