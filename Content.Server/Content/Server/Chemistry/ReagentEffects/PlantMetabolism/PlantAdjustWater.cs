using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067E RID: 1662
	public sealed class PlantAdjustWater : PlantAdjustAttribute
	{
		// Token: 0x060022AA RID: 8874 RVA: 0x000B493C File Offset: 0x000B2B3C
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, false))
			{
				return;
			}
			args.EntityManager.System<PlantHolderSystem>().AdjustWater(args.SolutionEntity, base.Amount, plantHolderComp);
		}
	}
}
