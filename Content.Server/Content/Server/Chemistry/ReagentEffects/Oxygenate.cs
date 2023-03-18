using System;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200066C RID: 1644
	public sealed class Oxygenate : ReagentEffect
	{
		// Token: 0x06002282 RID: 8834 RVA: 0x000B4294 File Offset: 0x000B2494
		public override void Effect(ReagentEffectArgs args)
		{
			RespiratorComponent resp;
			if (args.EntityManager.TryGetComponent<RespiratorComponent>(args.SolutionEntity, ref resp))
			{
				EntitySystem.Get<RespiratorSystem>().UpdateSaturation(resp.Owner, args.Quantity.Float() * this.Factor, resp);
			}
		}

		// Token: 0x04001555 RID: 5461
		[DataField("factor", false, 1, false, false, null)]
		public float Factor = 1f;
	}
}
