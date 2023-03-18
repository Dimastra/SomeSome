using System;
using Content.Shared.Alert;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000656 RID: 1622
	public sealed class AdjustAlert : ReagentEffect
	{
		// Token: 0x0600224A RID: 8778 RVA: 0x000B3780 File Offset: 0x000B1980
		public override void Effect(ReagentEffectArgs args)
		{
			AlertsSystem alertSys = EntitySystem.Get<AlertsSystem>();
			if (args.EntityManager.HasComponent<AlertsComponent>(args.SolutionEntity))
			{
				if (this.Clear)
				{
					alertSys.ClearAlert(args.SolutionEntity, this.Type);
					return;
				}
				ValueTuple<TimeSpan, TimeSpan>? cooldown = null;
				if (this.Cooldown)
				{
					IGameTiming timing = IoCManager.Resolve<IGameTiming>();
					cooldown = new ValueTuple<TimeSpan, TimeSpan>?(new ValueTuple<TimeSpan, TimeSpan>(timing.CurTime, timing.CurTime + TimeSpan.FromSeconds((double)this.Time)));
				}
				AlertsSystem alertsSystem = alertSys;
				EntityUid solutionEntity = args.SolutionEntity;
				AlertType type = this.Type;
				ValueTuple<TimeSpan, TimeSpan>? cooldown2 = cooldown;
				alertsSystem.ShowAlert(solutionEntity, type, null, cooldown2);
			}
		}

		// Token: 0x04001531 RID: 5425
		[DataField("alertType", false, 1, true, false, null)]
		public AlertType Type;

		// Token: 0x04001532 RID: 5426
		[DataField("clear", false, 1, false, false, null)]
		public bool Clear;

		// Token: 0x04001533 RID: 5427
		[DataField("cooldown", false, 1, false, false, null)]
		public bool Cooldown;

		// Token: 0x04001534 RID: 5428
		[DataField("time", false, 1, false, false, null)]
		public float Time;
	}
}
