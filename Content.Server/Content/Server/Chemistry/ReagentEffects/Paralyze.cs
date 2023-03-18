using System;
using Content.Server.Stunnable;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200066D RID: 1645
	public sealed class Paralyze : ReagentEffect
	{
		// Token: 0x06002284 RID: 8836 RVA: 0x000B42F4 File Offset: 0x000B24F4
		public override void Effect(ReagentEffectArgs args)
		{
			double paralyzeTime = this.ParalyzeTime;
			paralyzeTime *= (double)args.Scale;
			EntitySystem.Get<StunSystem>().TryParalyze(args.SolutionEntity, TimeSpan.FromSeconds(paralyzeTime), this.Refresh, null);
		}

		// Token: 0x04001556 RID: 5462
		[DataField("paralyzeTime", false, 1, false, false, null)]
		public double ParalyzeTime = 2.0;

		// Token: 0x04001557 RID: 5463
		[DataField("refresh", false, 1, false, false, null)]
		public bool Refresh = true;
	}
}
