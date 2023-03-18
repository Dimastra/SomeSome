using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Chemistry.ReagentEffectConditions
{
	// Token: 0x02000687 RID: 1671
	public sealed class OrganType : ReagentEffectCondition
	{
		// Token: 0x060022BC RID: 8892 RVA: 0x000B4D9C File Offset: 0x000B2F9C
		public override bool Condition(ReagentEffectArgs args)
		{
			if (args.OrganEntity == null)
			{
				return false;
			}
			MetabolizerComponent metabolizer;
			if (args.EntityManager.TryGetComponent<MetabolizerComponent>(args.OrganEntity.Value, ref metabolizer) && metabolizer.MetabolizerTypes != null && metabolizer.MetabolizerTypes.Contains(this.Type))
			{
				return this.ShouldHave;
			}
			return !this.ShouldHave;
		}

		// Token: 0x04001578 RID: 5496
		[Nullable(1)]
		[DataField("type", false, 1, true, false, typeof(PrototypeIdSerializer<MetabolizerTypePrototype>))]
		public string Type;

		// Token: 0x04001579 RID: 5497
		[DataField("shouldHave", false, 1, false, false, null)]
		public bool ShouldHave = true;
	}
}
