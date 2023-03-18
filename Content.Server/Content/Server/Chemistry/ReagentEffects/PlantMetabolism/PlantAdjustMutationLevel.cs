using System;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000679 RID: 1657
	public sealed class PlantAdjustMutationLevel : PlantAdjustAttribute
	{
		// Token: 0x060022A0 RID: 8864 RVA: 0x000B47D4 File Offset: 0x000B29D4
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			plantHolderComp.MutationLevel += base.Amount * plantHolderComp.MutationMod;
		}
	}
}
