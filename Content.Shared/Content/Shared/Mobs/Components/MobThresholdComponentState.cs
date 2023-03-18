using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs.Components
{
	// Token: 0x02000306 RID: 774
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MobThresholdComponentState : ComponentState
	{
		// Token: 0x060008ED RID: 2285 RVA: 0x0001E2FB File Offset: 0x0001C4FB
		public MobThresholdComponentState(MobState currentThresholdState, Dictionary<FixedPoint2, MobState> thresholds)
		{
			this.CurrentThresholdState = currentThresholdState;
			this.Thresholds = thresholds;
		}

		// Token: 0x040008D4 RID: 2260
		public Dictionary<FixedPoint2, MobState> Thresholds;

		// Token: 0x040008D5 RID: 2261
		public MobState CurrentThresholdState;
	}
}
