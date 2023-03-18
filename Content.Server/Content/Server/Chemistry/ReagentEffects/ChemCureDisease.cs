using System;
using Content.Server.Disease;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065B RID: 1627
	public sealed class ChemCureDisease : ReagentEffect
	{
		// Token: 0x06002254 RID: 8788 RVA: 0x000B3AB8 File Offset: 0x000B1CB8
		public override void Effect(ReagentEffectArgs args)
		{
			CureDiseaseAttemptEvent ev = new CureDiseaseAttemptEvent(this.CureChance * args.Scale);
			args.EntityManager.EventBus.RaiseLocalEvent<CureDiseaseAttemptEvent>(args.SolutionEntity, ev, false);
		}

		// Token: 0x0400153C RID: 5436
		[DataField("cureChance", false, 1, false, false, null)]
		public float CureChance = 0.15f;
	}
}
