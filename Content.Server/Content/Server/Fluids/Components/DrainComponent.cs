using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F4 RID: 1268
	[RegisterComponent]
	public sealed class DrainComponent : Component
	{
		// Token: 0x04001074 RID: 4212
		[Nullable(1)]
		public const string SolutionName = "drainBuffer";

		// Token: 0x04001075 RID: 4213
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x04001076 RID: 4214
		[DataField("unitsPerSecond", false, 1, false, false, null)]
		public float UnitsPerSecond = 6f;

		// Token: 0x04001077 RID: 4215
		[DataField("unitsDestroyedPerSecond", false, 1, false, false, null)]
		public float UnitsDestroyedPerSecond = 1f;

		// Token: 0x04001078 RID: 4216
		[DataField("range", false, 1, false, false, null)]
		public float Range = 2f;

		// Token: 0x04001079 RID: 4217
		[DataField("drainFrequency", false, 1, false, false, null)]
		public float DrainFrequency = 1f;
	}
}
