using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Kudzu
{
	// Token: 0x0200042A RID: 1066
	[RegisterComponent]
	public sealed class GrowingKudzuComponent : Component
	{
		// Token: 0x04000D68 RID: 3432
		[DataField("growthLevel", false, 1, false, false, null)]
		public int GrowthLevel = 1;

		// Token: 0x04000D69 RID: 3433
		[DataField("growthTickSkipChance", false, 1, false, false, null)]
		public float GrowthTickSkipChange;
	}
}
