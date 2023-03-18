using System;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000689 RID: 1673
	public sealed class SolutionTemperature : ReagentEffectCondition
	{
		// Token: 0x060022C0 RID: 8896 RVA: 0x000B4EB5 File Offset: 0x000B30B5
		public override bool Condition(ReagentEffectArgs args)
		{
			return args.Source != null && args.Source.Temperature >= this.Min && args.Source.Temperature <= this.Max;
		}

		// Token: 0x0400157D RID: 5501
		[DataField("min", false, 1, false, false, null)]
		public float Min;

		// Token: 0x0400157E RID: 5502
		[DataField("max", false, 1, false, false, null)]
		public float Max = float.PositiveInfinity;
	}
}
