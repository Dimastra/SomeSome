using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds
{
	// Token: 0x0200059A RID: 1434
	[DataDefinition]
	[Serializable]
	public struct MinMax
	{
		// Token: 0x04001331 RID: 4913
		[DataField("min", false, 1, false, false, null)]
		public int Min;

		// Token: 0x04001332 RID: 4914
		[DataField("max", false, 1, false, false, null)]
		public int Max;
	}
}
