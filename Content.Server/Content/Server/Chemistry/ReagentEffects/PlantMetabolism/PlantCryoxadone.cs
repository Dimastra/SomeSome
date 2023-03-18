using System;
using Content.Server.Botany;
using Content.Server.Botany.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.PlantMetabolism
{
	// Token: 0x02000681 RID: 1665
	[DataDefinition]
	public sealed class PlantCryoxadone : ReagentEffect
	{
		// Token: 0x060022B0 RID: 8880 RVA: 0x000B4A20 File Offset: 0x000B2C20
		public override void Effect(ReagentEffectArgs args)
		{
			PlantHolderComponent plantHolderComp;
			if (!args.EntityManager.TryGetComponent<PlantHolderComponent>(args.SolutionEntity, ref plantHolderComp) || plantHolderComp.Seed == null || plantHolderComp.Dead)
			{
				return;
			}
			SeedData seed = plantHolderComp.Seed;
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			int deviation;
			if ((float)plantHolderComp.Age > seed.Maturation)
			{
				deviation = (int)Math.Max(seed.Maturation - 1f, (float)(plantHolderComp.Age - random.Next(7, 10)));
			}
			else
			{
				deviation = (int)(seed.Maturation / (float)seed.GrowthStages);
			}
			plantHolderComp.Age -= deviation;
			PlantHolderComponent plantHolderComponent = plantHolderComp;
			int skipAging = plantHolderComponent.SkipAging;
			plantHolderComponent.SkipAging = skipAging + 1;
			plantHolderComp.ForceUpdate = true;
		}
	}
}
