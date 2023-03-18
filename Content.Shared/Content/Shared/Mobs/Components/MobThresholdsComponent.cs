using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Mobs.Components
{
	// Token: 0x02000305 RID: 773
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(MobThresholdSystem)
	})]
	public sealed class MobThresholdsComponent : Component
	{
		// Token: 0x040008D1 RID: 2257
		[Nullable(1)]
		[DataField("thresholds", false, 1, true, false, null)]
		public SortedDictionary<FixedPoint2, MobState> Thresholds = new SortedDictionary<FixedPoint2, MobState>();

		// Token: 0x040008D2 RID: 2258
		[DataField("triggersAlerts", false, 1, false, false, null)]
		public bool TriggersAlerts = true;

		// Token: 0x040008D3 RID: 2259
		public MobState CurrentThresholdState;
	}
}
