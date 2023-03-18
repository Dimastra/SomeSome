using System;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000668 RID: 1640
	public sealed class ModifyBleedAmount : ReagentEffect
	{
		// Token: 0x06002275 RID: 8821 RVA: 0x000B3FB8 File Offset: 0x000B21B8
		public override void Effect(ReagentEffectArgs args)
		{
			BloodstreamComponent blood;
			if (args.EntityManager.TryGetComponent<BloodstreamComponent>(args.SolutionEntity, ref blood))
			{
				BloodstreamSystem bloodstreamSystem = EntitySystem.Get<BloodstreamSystem>();
				float amt = this.Scaled ? (this.Amount * args.Quantity.Float()) : this.Amount;
				amt *= args.Scale;
				bloodstreamSystem.TryModifyBleedAmount(args.SolutionEntity, amt, blood);
			}
		}

		// Token: 0x0400154D RID: 5453
		[DataField("scaled", false, 1, false, false, null)]
		public bool Scaled;

		// Token: 0x0400154E RID: 5454
		[DataField("amount", false, 1, false, false, null)]
		public float Amount = -1f;
	}
}
