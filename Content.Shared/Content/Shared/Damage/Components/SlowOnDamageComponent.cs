using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000544 RID: 1348
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class SlowOnDamageComponent : Component
	{
		// Token: 0x04000F70 RID: 3952
		[Nullable(1)]
		[DataField("speedModifierThresholds", false, 1, true, false, null)]
		public readonly Dictionary<FixedPoint2, float> SpeedModifierThresholds;
	}
}
