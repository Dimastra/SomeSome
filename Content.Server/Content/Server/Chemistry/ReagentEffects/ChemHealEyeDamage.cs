using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Eye.Blinding;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065C RID: 1628
	public sealed class ChemHealEyeDamage : ReagentEffect
	{
		// Token: 0x06002256 RID: 8790 RVA: 0x000B3B06 File Offset: 0x000B1D06
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Scale != 1f)
			{
				return;
			}
			args.EntityManager.EntitySysManager.GetEntitySystem<SharedBlindingSystem>().AdjustEyeDamage(args.SolutionEntity, this.Amount, null);
		}

		// Token: 0x0400153D RID: 5437
		[DataField("amount", false, 1, false, false, null)]
		public int Amount = -1;
	}
}
