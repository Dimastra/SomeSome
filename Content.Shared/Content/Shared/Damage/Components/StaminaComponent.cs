using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000545 RID: 1349
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StaminaComponent : Component
	{
		// Token: 0x04000F71 RID: 3953
		[ViewVariables]
		[DataField("critical", false, 1, false, false, null)]
		public bool Critical;

		// Token: 0x04000F72 RID: 3954
		[ViewVariables]
		[DataField("decay", false, 1, false, false, null)]
		public float Decay = 3f;

		// Token: 0x04000F73 RID: 3955
		[ViewVariables]
		[DataField("cooldown", false, 1, false, false, null)]
		public float DecayCooldown = 5f;

		// Token: 0x04000F74 RID: 3956
		[ViewVariables]
		[DataField("staminaDamage", false, 1, false, false, null)]
		public float StaminaDamage;

		// Token: 0x04000F75 RID: 3957
		[ViewVariables]
		[DataField("excess", false, 1, false, false, null)]
		public float CritThreshold = 100f;

		// Token: 0x04000F76 RID: 3958
		[DataField("lastUpdate", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextUpdate = TimeSpan.Zero;
	}
}
