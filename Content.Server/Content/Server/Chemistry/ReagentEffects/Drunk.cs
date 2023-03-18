using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Drunk;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000660 RID: 1632
	public sealed class Drunk : ReagentEffect
	{
		// Token: 0x06002260 RID: 8800 RVA: 0x000B3C54 File Offset: 0x000B1E54
		public override void Effect(ReagentEffectArgs args)
		{
			float boozePower = this.BoozePower;
			boozePower *= args.Scale;
			args.EntityManager.EntitySysManager.GetEntitySystem<SharedDrunkSystem>().TryApplyDrunkenness(args.SolutionEntity, boozePower, this.SlurSpeech, null);
		}

		// Token: 0x04001542 RID: 5442
		[DataField("boozePower", false, 1, false, false, null)]
		public float BoozePower = 2f;

		// Token: 0x04001543 RID: 5443
		[DataField("slurSpeech", false, 1, false, false, null)]
		public bool SlurSpeech = true;
	}
}
