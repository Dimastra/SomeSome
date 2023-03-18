using System;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000669 RID: 1641
	public sealed class ModifyBloodLevel : ReagentEffect
	{
		// Token: 0x06002277 RID: 8823 RVA: 0x000B4034 File Offset: 0x000B2234
		public override void Effect(ReagentEffectArgs args)
		{
			BloodstreamComponent blood;
			if (args.EntityManager.TryGetComponent<BloodstreamComponent>(args.SolutionEntity, ref blood))
			{
				BloodstreamSystem bloodstreamSystem = EntitySystem.Get<BloodstreamSystem>();
				FixedPoint2 amt = this.Scaled ? (this.Amount * args.Quantity) : this.Amount;
				amt *= args.Scale;
				bloodstreamSystem.TryModifyBloodLevel(args.SolutionEntity, amt, blood);
			}
		}

		// Token: 0x0400154F RID: 5455
		[DataField("scaled", false, 1, false, false, null)]
		public bool Scaled;

		// Token: 0x04001550 RID: 5456
		[DataField("amount", false, 1, false, false, null)]
		public FixedPoint2 Amount = 1f;
	}
}
