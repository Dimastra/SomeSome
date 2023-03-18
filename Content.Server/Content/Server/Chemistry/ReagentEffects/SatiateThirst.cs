using System;
using Content.Server.Nutrition.Components;
using Content.Server.Nutrition.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000672 RID: 1650
	public sealed class SatiateThirst : ReagentEffect
	{
		// Token: 0x17000533 RID: 1331
		// (get) Token: 0x0600228E RID: 8846 RVA: 0x000B44D1 File Offset: 0x000B26D1
		// (set) Token: 0x0600228F RID: 8847 RVA: 0x000B44D9 File Offset: 0x000B26D9
		[DataField("factor", false, 1, false, false, null)]
		public float HydrationFactor { get; set; } = 3f;

		// Token: 0x06002290 RID: 8848 RVA: 0x000B44E4 File Offset: 0x000B26E4
		public override void Effect(ReagentEffectArgs args)
		{
			ThirstComponent thirst;
			if (args.EntityManager.TryGetComponent<ThirstComponent>(args.SolutionEntity, ref thirst))
			{
				EntitySystem.Get<ThirstSystem>().UpdateThirst(thirst, this.HydrationFactor);
			}
		}
	}
}
