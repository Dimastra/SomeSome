using System;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x02000693 RID: 1683
	public sealed class ChemCleanBloodstream : ReagentEffect
	{
		// Token: 0x060022DB RID: 8923 RVA: 0x000B54AC File Offset: 0x000B36AC
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Source == null)
			{
				return;
			}
			float cleanseRate = this.CleanseRate;
			cleanseRate *= args.Scale;
			EntitySystem.Get<BloodstreamSystem>().FlushChemicals(args.SolutionEntity, args.Reagent.ID, cleanseRate, null);
		}

		// Token: 0x0400159C RID: 5532
		[DataField("cleanseRate", false, 1, false, false, null)]
		public float CleanseRate = 3f;
	}
}
