using System;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000673 RID: 1651
	public sealed class WashCreamPieReaction : ReagentEffect
	{
		// Token: 0x06002292 RID: 8850 RVA: 0x000B452C File Offset: 0x000B272C
		public override void Effect(ReagentEffectArgs args)
		{
			CreamPiedComponent creamPied;
			if (!args.EntityManager.TryGetComponent<CreamPiedComponent>(args.SolutionEntity, ref creamPied))
			{
				return;
			}
			EntitySystem.Get<CreamPieSystem>().SetCreamPied(args.SolutionEntity, creamPied, false);
		}
	}
}
