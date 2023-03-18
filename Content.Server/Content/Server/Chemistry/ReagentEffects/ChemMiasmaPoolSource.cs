using System;
using Content.Server.Atmos.Miasma;
using Content.Server.Disease;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200065D RID: 1629
	public sealed class ChemMiasmaPoolSource : ReagentEffect
	{
		// Token: 0x06002258 RID: 8792 RVA: 0x000B3B4C File Offset: 0x000B1D4C
		public override void Effect(ReagentEffectArgs args)
		{
			if (args.Scale != 1f)
			{
				return;
			}
			string disease = EntitySystem.Get<MiasmaSystem>().RequestPoolDisease();
			EntitySystem.Get<DiseaseSystem>().TryAddDisease(args.SolutionEntity, disease, null);
		}
	}
}
