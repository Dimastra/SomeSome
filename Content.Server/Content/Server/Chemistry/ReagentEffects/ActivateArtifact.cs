using System;
using Content.Server.Xenoarchaeology.XenoArtifacts;
using Content.Shared.Chemistry.Reagent;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000654 RID: 1620
	public sealed class ActivateArtifact : ReagentEffect
	{
		// Token: 0x06002246 RID: 8774 RVA: 0x000B36A8 File Offset: 0x000B18A8
		public override void Effect(ReagentEffectArgs args)
		{
			args.EntityManager.EntitySysManager.GetEntitySystem<ArtifactSystem>().TryActivateArtifact(args.SolutionEntity, null, null);
		}
	}
}
