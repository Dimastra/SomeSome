using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.StatusEffect;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects.StatusEffects
{
	// Token: 0x02000674 RID: 1652
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GenericStatusEffect : ReagentEffect
	{
		// Token: 0x06002294 RID: 8852 RVA: 0x000B456C File Offset: 0x000B276C
		public override void Effect(ReagentEffectArgs args)
		{
			StatusEffectsSystem statusSys = args.EntityManager.EntitySysManager.GetEntitySystem<StatusEffectsSystem>();
			float time = this.Time;
			time *= args.Scale;
			if (this.Type == StatusEffectMetabolismType.Add && this.Component != string.Empty)
			{
				statusSys.TryAddStatusEffect(args.SolutionEntity, this.Key, TimeSpan.FromSeconds((double)time), this.Refresh, this.Component, null);
				return;
			}
			if (this.Type == StatusEffectMetabolismType.Remove)
			{
				statusSys.TryRemoveTime(args.SolutionEntity, this.Key, TimeSpan.FromSeconds((double)time), null);
				return;
			}
			if (this.Type == StatusEffectMetabolismType.Set)
			{
				statusSys.TrySetTime(args.SolutionEntity, this.Key, TimeSpan.FromSeconds((double)time), null);
			}
		}

		// Token: 0x04001561 RID: 5473
		[DataField("key", false, 1, true, false, null)]
		public string Key;

		// Token: 0x04001562 RID: 5474
		[DataField("component", false, 1, false, false, null)]
		public string Component = string.Empty;

		// Token: 0x04001563 RID: 5475
		[DataField("time", false, 1, false, false, null)]
		public float Time = 2f;

		// Token: 0x04001564 RID: 5476
		[DataField("refresh", false, 1, false, false, null)]
		public bool Refresh = true;

		// Token: 0x04001565 RID: 5477
		[DataField("type", false, 1, false, false, null)]
		public StatusEffectMetabolismType Type;
	}
}
