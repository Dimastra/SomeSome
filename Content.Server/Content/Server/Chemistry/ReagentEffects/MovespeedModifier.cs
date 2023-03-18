using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x0200066B RID: 1643
	public sealed class MovespeedModifier : ReagentEffect
	{
		// Token: 0x17000530 RID: 1328
		// (get) Token: 0x0600227B RID: 8827 RVA: 0x000B4164 File Offset: 0x000B2364
		// (set) Token: 0x0600227C RID: 8828 RVA: 0x000B416C File Offset: 0x000B236C
		[DataField("walkSpeedModifier", false, 1, false, false, null)]
		public float WalkSpeedModifier { get; set; } = 1f;

		// Token: 0x17000531 RID: 1329
		// (get) Token: 0x0600227D RID: 8829 RVA: 0x000B4175 File Offset: 0x000B2375
		// (set) Token: 0x0600227E RID: 8830 RVA: 0x000B417D File Offset: 0x000B237D
		[DataField("sprintSpeedModifier", false, 1, false, false, null)]
		public float SprintSpeedModifier { get; set; } = 1f;

		// Token: 0x0600227F RID: 8831 RVA: 0x000B4188 File Offset: 0x000B2388
		public override void Effect(ReagentEffectArgs args)
		{
			MovespeedModifierMetabolismComponent status = args.EntityManager.EnsureComponent<MovespeedModifierMetabolismComponent>(args.SolutionEntity);
			bool flag = !status.WalkSpeedModifier.Equals(this.WalkSpeedModifier) || !status.SprintSpeedModifier.Equals(this.SprintSpeedModifier);
			status.WalkSpeedModifier = this.WalkSpeedModifier;
			status.SprintSpeedModifier = this.SprintSpeedModifier;
			float statusLifetime = this.StatusLifetime;
			statusLifetime *= args.Scale;
			this.IncreaseTimer(status, statusLifetime);
			if (flag)
			{
				EntitySystem.Get<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(args.SolutionEntity, null);
			}
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x000B4220 File Offset: 0x000B2420
		[NullableContext(1)]
		public void IncreaseTimer(MovespeedModifierMetabolismComponent status, float time)
		{
			IGameTiming gameTiming = IoCManager.Resolve<IGameTiming>();
			double offsetTime = Math.Max(status.ModifierTimer.TotalSeconds, gameTiming.CurTime.TotalSeconds);
			status.ModifierTimer = TimeSpan.FromSeconds(offsetTime + (double)time);
			status.Dirty(null);
		}

		// Token: 0x04001554 RID: 5460
		[DataField("statusLifetime", false, 1, false, false, null)]
		public float StatusLifetime = 2f;
	}
}
