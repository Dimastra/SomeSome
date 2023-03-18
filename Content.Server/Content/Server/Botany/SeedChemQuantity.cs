using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Botany
{
	// Token: 0x020006F8 RID: 1784
	[DataDefinition]
	public struct SeedChemQuantity
	{
		// Token: 0x040016E6 RID: 5862
		[DataField("Min", false, 1, false, false, null)]
		public int Min;

		// Token: 0x040016E7 RID: 5863
		[DataField("Max", false, 1, false, false, null)]
		public int Max;

		// Token: 0x040016E8 RID: 5864
		[DataField("PotencyDivisor", false, 1, false, false, null)]
		public int PotencyDivisor;
	}
}
