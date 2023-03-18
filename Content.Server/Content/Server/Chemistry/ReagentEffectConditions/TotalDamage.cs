using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x0200068A RID: 1674
	public sealed class TotalDamage : ReagentEffectCondition
	{
		// Token: 0x060022C2 RID: 8898 RVA: 0x000B4F04 File Offset: 0x000B3104
		public override bool Condition(ReagentEffectArgs args)
		{
			DamageableComponent damage;
			if (args.EntityManager.TryGetComponent<DamageableComponent>(args.SolutionEntity, ref damage))
			{
				FixedPoint2 total = damage.TotalDamage;
				if (total > this.Min && total < this.Max)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400157F RID: 5503
		[DataField("max", false, 1, false, false, null)]
		public FixedPoint2 Max = FixedPoint2.MaxValue;

		// Token: 0x04001580 RID: 5504
		[DataField("min", false, 1, false, false, null)]
		public FixedPoint2 Min = FixedPoint2.Zero;
	}
}
