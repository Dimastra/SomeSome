using System;
using Content.Server.Nutrition.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000671 RID: 1649
	public sealed class SatiateHunger : ReagentEffect
	{
		// Token: 0x17000532 RID: 1330
		// (get) Token: 0x0600228A RID: 8842 RVA: 0x000B446D File Offset: 0x000B266D
		// (set) Token: 0x0600228B RID: 8843 RVA: 0x000B4475 File Offset: 0x000B2675
		[DataField("factor", false, 1, false, false, null)]
		public float NutritionFactor { get; set; } = 3f;

		// Token: 0x0600228C RID: 8844 RVA: 0x000B4480 File Offset: 0x000B2680
		public override void Effect(ReagentEffectArgs args)
		{
			HungerComponent hunger;
			if (args.EntityManager.TryGetComponent<HungerComponent>(args.SolutionEntity, ref hunger))
			{
				hunger.UpdateFood(this.NutritionFactor * (float)args.Quantity);
			}
		}
	}
}
