using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Random
{
	// Token: 0x02000219 RID: 537
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("weightedRandom", 1)]
	public sealed class WeightedRandomPrototype : IPrototype
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x00015119 File Offset: 0x00013319
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000600 RID: 1536
		[DataField("weights", false, 1, false, false, null)]
		public Dictionary<string, float> Weights = new Dictionary<string, float>();
	}
}
