using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000686 RID: 1670
	public sealed class MobStateCondition : ReagentEffectCondition
	{
		// Token: 0x060022BA RID: 8890 RVA: 0x000B4D58 File Offset: 0x000B2F58
		public override bool Condition(ReagentEffectArgs args)
		{
			MobStateComponent mobState;
			return args.EntityManager.TryGetComponent<MobStateComponent>(args.SolutionEntity, ref mobState) && mobState.CurrentState == this.mobstate;
		}

		// Token: 0x04001577 RID: 5495
		[DataField("mobstate", false, 1, false, false, null)]
		public MobState mobstate = MobState.Alive;
	}
}
