using System;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Database;
using Robust.Shared.GameObjects;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000666 RID: 1638
	public sealed class Ignite : ReagentEffect
	{
		// Token: 0x1700052E RID: 1326
		// (get) Token: 0x0600226F RID: 8815 RVA: 0x000B3F1E File Offset: 0x000B211E
		public override bool ShouldLog
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700052F RID: 1327
		// (get) Token: 0x06002270 RID: 8816 RVA: 0x000B3F21 File Offset: 0x000B2121
		public override LogImpact LogImpact
		{
			get
			{
				return LogImpact.Medium;
			}
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x000B3F24 File Offset: 0x000B2124
		public override void Effect(ReagentEffectArgs args)
		{
			EntitySystem.Get<FlammableSystem>().Ignite(args.SolutionEntity, null);
		}
	}
}
