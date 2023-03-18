using System;
using System.Runtime.CompilerServices;
using Content.Server.Disease;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000659 RID: 1625
	public sealed class ChemCauseDisease : ReagentEffect
	{
		// Token: 0x06002250 RID: 8784 RVA: 0x000B3A19 File Offset: 0x000B1C19
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Scale != 1f)
			{
				return;
			}
			EntitySystem.Get<DiseaseSystem>().TryAddDisease(args.SolutionEntity, this.Disease, null);
		}

		// Token: 0x04001539 RID: 5433
		[DataField("causeChance", false, 1, false, false, null)]
		public float CauseChance = 0.15f;

		// Token: 0x0400153A RID: 5434
		[Nullable(1)]
		[DataField("disease", false, 1, true, false, typeof(PrototypeIdSerializer<DiseasePrototype>))]
		[ViewVariables]
		public string Disease;
	}
}
