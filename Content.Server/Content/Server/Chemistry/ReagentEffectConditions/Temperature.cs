using System;
using Content.Server.Temperature.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000684 RID: 1668
	public sealed class Temperature : ReagentEffectCondition
	{
		// Token: 0x060022B6 RID: 8886 RVA: 0x000B4CB8 File Offset: 0x000B2EB8
		public override bool Condition(ReagentEffectArgs args)
		{
			TemperatureComponent temp;
			return args.EntityManager.TryGetComponent<TemperatureComponent>(args.SolutionEntity, ref temp) && temp.CurrentTemperature > this.Min && temp.CurrentTemperature < this.Max;
		}

		// Token: 0x04001573 RID: 5491
		[DataField("min", false, 1, false, false, null)]
		public float Min;

		// Token: 0x04001574 RID: 5492
		[DataField("max", false, 1, false, false, null)]
		public float Max = float.MaxValue;
	}
}
