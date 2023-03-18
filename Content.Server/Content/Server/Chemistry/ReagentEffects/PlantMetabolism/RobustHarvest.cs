using System;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000683 RID: 1667
	[DataDefinition]
	public sealed class RobustHarvest : ReagentEffect
	{
		// Token: 0x060022B4 RID: 8884 RVA: 0x000B4B98 File Offset: 0x000B2D98
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!args.EntityManager.TryGetComponent<PlantHolderComponent>(args.SolutionEntity, ref plantHolderComp) || plantHolderComp.Seed == null || plantHolderComp.Dead || plantHolderComp.Seed.Immutable)
			{
				return;
			}
			PlantHolderSystem plantHolder = args.EntityManager.System<PlantHolderSystem>();
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			if (plantHolderComp.Seed.Potency < (float)this.PotencyLimit)
			{
				plantHolder.EnsureUniqueSeed(args.SolutionEntity, plantHolderComp);
				plantHolderComp.Seed.Potency = Math.Min(plantHolderComp.Seed.Potency + (float)this.PotencyIncrease, (float)this.PotencyLimit);
				if (plantHolderComp.Seed.Potency > (float)this.PotencySeedlessThreshold)
				{
					plantHolderComp.Seed.Seedless = true;
					return;
				}
			}
			else if (plantHolderComp.Seed.Yield > 1 && RandomExtensions.Prob(random, 0.1f))
			{
				plantHolder.EnsureUniqueSeed(args.SolutionEntity, plantHolderComp);
				plantHolderComp.Seed.Yield--;
			}
		}

		// Token: 0x04001570 RID: 5488
		[DataField("potencyLimit", false, 1, false, false, null)]
		public int PotencyLimit = 50;

		// Token: 0x04001571 RID: 5489
		[DataField("potencyIncrease", false, 1, false, false, null)]
		public int PotencyIncrease = 3;

		// Token: 0x04001572 RID: 5490
		[DataField("potencySeedlessThreshold", false, 1, false, false, null)]
		public int PotencySeedlessThreshold = 30;
	}
}
