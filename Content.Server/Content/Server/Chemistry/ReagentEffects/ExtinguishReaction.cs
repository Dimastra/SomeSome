using System;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000663 RID: 1635
	public sealed class ExtinguishReaction : ReagentEffect
	{
		// Token: 0x06002267 RID: 8807 RVA: 0x000B3DB8 File Offset: 0x000B1FB8
		public override void Effect(ReagentEffectArgs args)
		{
			FlammableComponent flammable;
			if (!args.EntityManager.TryGetComponent<FlammableComponent>(args.SolutionEntity, ref flammable))
			{
				return;
			}
			FlammableSystem flammableSystem = EntitySystem.Get<FlammableSystem>();
			flammableSystem.Extinguish(args.SolutionEntity, flammable);
			flammableSystem.AdjustFireStacks(args.SolutionEntity, -1.5f * (float)args.Quantity, flammable);
		}
	}
}
