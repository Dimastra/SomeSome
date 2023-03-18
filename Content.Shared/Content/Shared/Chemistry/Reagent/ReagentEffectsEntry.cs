using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chemistry.Reagent
{
	// Token: 0x020005E8 RID: 1512
	[DataDefinition]
	public sealed class ReagentEffectsEntry
	{
		// Token: 0x04001126 RID: 4390
		[JsonPropertyName("rate")]
		[DataField("metabolismRate", false, 1, false, false, null)]
		public FixedPoint2 MetabolismRate = FixedPoint2.New(0.5f);

		// Token: 0x04001127 RID: 4391
		[Nullable(1)]
		[JsonPropertyName("effects")]
		[DataField("effects", false, 1, true, false, null)]
		public ReagentEffect[] Effects;
	}
}
