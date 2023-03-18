using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067B RID: 1659
	public sealed class PlantAdjustNutrition : PlantAdjustAttribute
	{
		// Token: 0x060022A4 RID: 8868 RVA: 0x000B4864 File Offset: 0x000B2A64
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, false))
			{
				return;
			}
			args.EntityManager.System<PlantHolderSystem>().AdjustNutrient(args.SolutionEntity, base.Amount, plantHolderComp);
		}
	}
}
