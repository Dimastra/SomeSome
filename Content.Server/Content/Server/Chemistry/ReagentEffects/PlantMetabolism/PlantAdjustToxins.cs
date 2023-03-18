using System;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067D RID: 1661
	public sealed class PlantAdjustToxins : PlantAdjustAttribute
	{
		// Token: 0x060022A8 RID: 8872 RVA: 0x000B48F8 File Offset: 0x000B2AF8
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			plantHolderComp.Toxins += base.Amount;
		}
	}
}
