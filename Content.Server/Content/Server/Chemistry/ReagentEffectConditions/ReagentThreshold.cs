using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000688 RID: 1672
	public sealed class ReagentThreshold : ReagentEffectCondition
	{
		// Token: 0x060022BE RID: 8894 RVA: 0x000B4E18 File Offset: 0x000B3018
		public override bool Condition(ReagentEffectArgs args)
		{
			if (this.Reagent == null)
			{
				this.Reagent = args.Reagent.ID;
			}
			FixedPoint2 quant = FixedPoint2.Zero;
			if (args.Source != null && args.Source.ContainsReagent(this.Reagent))
			{
				quant = args.Source.GetReagentQuantity(args.Reagent.ID);
			}
			return quant >= this.Min && quant <= this.Max;
		}

		// Token: 0x0400157A RID: 5498
		[DataField("min", false, 1, false, false, null)]
		public FixedPoint2 Min = FixedPoint2.Zero;

		// Token: 0x0400157B RID: 5499
		[DataField("max", false, 1, false, false, null)]
		public FixedPoint2 Max = FixedPoint2.MaxValue;

		// Token: 0x0400157C RID: 5500
		[Nullable(2)]
		[DataField("reagent", false, 1, false, false, null)]
		public string Reagent;
	}
}
