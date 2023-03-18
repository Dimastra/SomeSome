using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000682 RID: 1666
	[DataDefinition]
	public sealed class PlantDiethylamine : ReagentEffect
	{
		// Token: 0x060022B2 RID: 8882 RVA: 0x000B4AD8 File Offset: 0x000B2CD8
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!args.EntityManager.TryGetComponent<PlantHolderComponent>(args.SolutionEntity, ref plantHolderComp) || plantHolderComp.Seed == null || plantHolderComp.Dead || plantHolderComp.Seed.Immutable)
			{
				return;
			}
			PlantHolderSystem plantHolder = args.EntityManager.System<PlantHolderSystem>();
			IRobustRandom robustRandom = IoCManager.Resolve<IRobustRandom>();
			if (RandomExtensions.Prob(robustRandom, 0.1f))
			{
				plantHolder.EnsureUniqueSeed(args.SolutionEntity, plantHolderComp);
				plantHolderComp.Seed.Lifespan += 1f;
			}
			if (RandomExtensions.Prob(robustRandom, 0.1f))
			{
				plantHolder.EnsureUniqueSeed(args.SolutionEntity, plantHolderComp);
				plantHolderComp.Seed.Endurance += 1f;
			}
		}
	}
}
