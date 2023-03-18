using System;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x0200067C RID: 1660
	public sealed class PlantAdjustPests : PlantAdjustAttribute
	{
		// Token: 0x060022A6 RID: 8870 RVA: 0x000B48B4 File Offset: 0x000B2AB4
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			plantHolderComp.PestLevel += base.Amount;
		}
	}
}
