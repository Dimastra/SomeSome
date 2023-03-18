using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Gravity
{
	// Token: 0x02000447 RID: 1095
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class GravityShakeComponent : Component
	{
		// Token: 0x04000CD0 RID: 3280
		[ViewVariables]
		[DataField("shakeTimes", false, 1, false, false, null)]
		public int ShakeTimes;

		// Token: 0x04000CD1 RID: 3281
		[DataField("nextShake", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextShake;
	}
}
