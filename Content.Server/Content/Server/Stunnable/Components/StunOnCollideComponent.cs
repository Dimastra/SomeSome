using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Stunnable.Components
{
	// Token: 0x0200014F RID: 335
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(StunOnCollideSystem)
	})]
	public sealed class StunOnCollideComponent : Component
	{
		// Token: 0x040003B2 RID: 946
		[DataField("stunAmount", false, 1, false, false, null)]
		public int StunAmount;

		// Token: 0x040003B3 RID: 947
		[DataField("knockdownAmount", false, 1, false, false, null)]
		public int KnockdownAmount;

		// Token: 0x040003B4 RID: 948
		[DataField("slowdownAmount", false, 1, false, false, null)]
		public int SlowdownAmount;

		// Token: 0x040003B5 RID: 949
		[DataField("walkSpeedMultiplier", false, 1, false, false, null)]
		public float WalkSpeedMultiplier = 1f;

		// Token: 0x040003B6 RID: 950
		[DataField("runSpeedMultiplier", false, 1, false, false, null)]
		public float RunSpeedMultiplier = 1f;

		// Token: 0x040003B7 RID: 951
		[Nullable(1)]
		[DataField("fixture", false, 1, false, false, null)]
		public string FixtureID = "projectile";
	}
}
