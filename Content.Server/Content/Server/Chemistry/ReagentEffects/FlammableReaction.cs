using System;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000664 RID: 1636
	public sealed class FlammableReaction : ReagentEffect
	{
		// Token: 0x1700052C RID: 1324
		// (get) Token: 0x06002269 RID: 8809 RVA: 0x000B3E18 File Offset: 0x000B2018
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700052D RID: 1325
		// (get) Token: 0x0600226A RID: 8810 RVA: 0x000B3E1B File Offset: 0x000B201B
		public override LogImpact LogImpact
		{
			get
			{
				return LogImpact.Medium;
			}
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x000B3E20 File Offset: 0x000B2020
		public override void Effect(ReagentEffectArgs args)
		{
			FlammableComponent flammable;
			if (!args.EntityManager.TryGetComponent<FlammableComponent>(args.SolutionEntity, ref flammable))
			{
				return;
			}
			EntitySystem.Get<FlammableSystem>().AdjustFireStacks(args.SolutionEntity, args.Quantity.Float() * this.Multiplier, flammable);
			Solution source = args.Source;
			if (source == null)
			{
				return;
			}
			source.RemoveReagent(args.Reagent.ID, args.Quantity);
		}

		// Token: 0x04001549 RID: 5449
		[DataField("multiplier", false, 1, false, false, null)]
		public float Multiplier = 0.05f;
	}
}
