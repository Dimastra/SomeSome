using System;
using Content.Server.Medical;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065E RID: 1630
	public sealed class ChemVomit : ReagentEffect
	{
		// Token: 0x0600225A RID: 8794 RVA: 0x000B3B8E File Offset: 0x000B1D8E
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Scale != 1f)
			{
				return;
			}
			args.EntityManager.EntitySysManager.GetEntitySystem<VomitSystem>().Vomit(args.SolutionEntity, this.ThirstAmount, this.HungerAmount);
		}

		// Token: 0x0400153E RID: 5438
		[DataField("thirstAmount", false, 1, false, false, null)]
		public float ThirstAmount = -40f;

		// Token: 0x0400153F RID: 5439
		[DataField("hungerAmount", false, 1, false, false, null)]
		public float HungerAmount = -40f;
	}
}
