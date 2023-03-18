using System;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067A RID: 1658
	public sealed class PlantAdjustMutationMod : PlantAdjustAttribute
	{
		// Token: 0x060022A2 RID: 8866 RVA: 0x000B4820 File Offset: 0x000B2A20
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			plantHolderComp.MutationMod += base.Amount;
		}
	}
}
