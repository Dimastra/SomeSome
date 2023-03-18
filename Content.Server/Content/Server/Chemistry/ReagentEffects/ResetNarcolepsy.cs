using System;
using Content.Server.Traits.Assorted;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000670 RID: 1648
	public sealed class ResetNarcolepsy : ReagentEffect
	{
		// Token: 0x06002288 RID: 8840 RVA: 0x000B4425 File Offset: 0x000B2625
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Scale != 1f)
			{
				return;
			}
			args.EntityManager.EntitySysManager.GetEntitySystem<NarcolepsySystem>().AdjustNarcolepsyTimer(args.SolutionEntity, this.TimerReset, null);
		}

		// Token: 0x0400155E RID: 5470
		[DataField("TimerReset", false, 1, false, false, null)]
		public int TimerReset = 600;
	}
}
