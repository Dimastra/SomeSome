using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000680 RID: 1664
	public sealed class PlantAffectGrowth : PlantAdjustAttribute
	{
		// Token: 0x060022AE RID: 8878 RVA: 0x000B49D0 File Offset: 0x000B2BD0
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!base.CanMetabolize(args.SolutionEntity, out plantHolderComp, args.EntityManager, true))
			{
				return;
			}
			args.EntityManager.System<PlantHolderSystem>().AffectGrowth(args.SolutionEntity, (int)base.Amount, plantHolderComp);
		}
	}
}
