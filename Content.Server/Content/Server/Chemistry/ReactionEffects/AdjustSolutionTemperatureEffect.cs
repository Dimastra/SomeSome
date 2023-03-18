using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x02000691 RID: 1681
	[DataDefinition]
	public sealed class AdjustSolutionTemperatureEffect : ReagentEffect
	{
		// Token: 0x060022D7 RID: 8919 RVA: 0x000B5368 File Offset: 0x000B3568
		public override void Effect(ReagentEffectArgs args)
		{
			Solution solution = args.Source;
			if (solution == null || solution.Volume == 0)
			{
				return;
			}
			float deltaT = this._scaled ? (this._delta * (float)args.Quantity) : this._delta;
			solution.Temperature = Math.Clamp(solution.Temperature + deltaT, this._minTemp, this._maxTemp);
		}

		// Token: 0x04001594 RID: 5524
		[DataField("delta", false, 1, true, false, null)]
		private float _delta;

		// Token: 0x04001595 RID: 5525
		[DataField("minTemp", false, 1, false, false, null)]
		private float _minTemp;

		// Token: 0x04001596 RID: 5526
		[DataField("maxTemp", false, 1, false, false, null)]
		private float _maxTemp = float.PositiveInfinity;

		// Token: 0x04001597 RID: 5527
		[DataField("scaled", false, 1, false, false, null)]
		private bool _scaled;
	}
}
