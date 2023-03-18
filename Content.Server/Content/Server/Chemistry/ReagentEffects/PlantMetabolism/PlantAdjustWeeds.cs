using System;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067F RID: 1663
	public sealed class PlantAdjustWeeds : PlantAdjustAttribute
	{
		// Token: 0x060022AC RID: 8876 RVA: 0x000B498C File Offset: 0x000B2B8C
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			plantHolderComp.WeedLevel += base.Amount;
		}
	}
}
