using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Cargo.Components
{
	// Token: 0x020006EA RID: 1770
	[RegisterComponent]
	public sealed class MobPriceComponent : Component
	{
		// Token: 0x040016C1 RID: 5825
		[DataField("missingBodyPartPenalty", false, 1, false, false, null)]
		public double MissingBodyPartPenalty = 1.0;

		// Token: 0x040016C2 RID: 5826
		[DataField("price", false, 1, true, false, null)]
		public double Price;

		// Token: 0x040016C3 RID: 5827
		[DataField("deathPenalty", false, 1, false, false, null)]
		public double DeathPenalty = 0.20000000298023224;
	}
}
