using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000665 RID: 1637
	public sealed class HealthChange : ReagentEffect
	{
		// Token: 0x0600226D RID: 8813 RVA: 0x000B3EA8 File Offset: 0x000B20A8
		public override void Effect(ReagentEffectArgs args)
		{
			FixedPoint2 scale = this.ScaleByQuantity ? args.Quantity : FixedPoint2.New(1);
			scale *= args.Scale;
			EntitySystem.Get<DamageableSystem>().TryChangeDamage(new EntityUid?(args.SolutionEntity), this.Damage * scale, this.IgnoreResistances, true, null, null);
		}

		// Token: 0x0400154A RID: 5450
		[Nullable(1)]
		[JsonPropertyName("damage")]
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier Damage;

		// Token: 0x0400154B RID: 5451
		[JsonPropertyName("scaleByQuantity")]
		[DataField("scaleByQuantity", false, 1, false, false, null)]
		public bool ScaleByQuantity;

		// Token: 0x0400154C RID: 5452
		[DataField("ignoreResistances", false, 1, false, false, null)]
		[JsonPropertyName("ignoreResistances")]
		public bool IgnoreResistances = true;
	}
}
