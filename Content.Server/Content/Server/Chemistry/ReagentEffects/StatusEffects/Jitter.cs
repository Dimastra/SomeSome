using System;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Jittering;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.StatusEffects
{
	// Token: 0x02000676 RID: 1654
	public sealed class Jitter : ReagentEffect
	{
		// Token: 0x06002296 RID: 8854 RVA: 0x000B4650 File Offset: 0x000B2850
		public override void Effect(ReagentEffectArgs args)
		{
			float time = this.Time;
			time *= args.Scale;
			args.EntityManager.EntitySysManager.GetEntitySystem<SharedJitteringSystem>().DoJitter(args.SolutionEntity, TimeSpan.FromSeconds((double)time), this.Refresh, this.Amplitude, this.Frequency, false, null);
		}

		// Token: 0x0400156A RID: 5482
		[DataField("amplitude", false, 1, false, false, null)]
		public float Amplitude = 10f;

		// Token: 0x0400156B RID: 5483
		[DataField("frequency", false, 1, false, false, null)]
		public float Frequency = 4f;

		// Token: 0x0400156C RID: 5484
		[DataField("time", false, 1, false, false, null)]
		public float Time = 2f;

		// Token: 0x0400156D RID: 5485
		[DataField("refresh", false, 1, false, false, null)]
		public bool Refresh = true;
	}
}
