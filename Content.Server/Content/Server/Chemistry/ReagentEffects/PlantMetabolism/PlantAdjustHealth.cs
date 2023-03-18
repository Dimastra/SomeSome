using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000678 RID: 1656
	public sealed class PlantAdjustHealth : PlantAdjustAttribute
	{
		// Token: 0x0600229E RID: 8862 RVA: 0x000B4778 File Offset: 0x000B2978
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			PlantHolderSystem plantHolderSystem = args.EntityManager.System<PlantHolderSystem>();
			plantHolderComp.Health += base.Amount;
			plantHolderSystem.CheckHealth(args.SolutionEntity, plantHolderComp);
		}
	}
}
