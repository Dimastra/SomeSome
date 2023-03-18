using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x02000692 RID: 1682
	public sealed class AdjustSolutionThermalEnergyEffect : ReagentEffect
	{
		// Token: 0x060022D9 RID: 8921 RVA: 0x000B53E8 File Offset: 0x000B35E8
		public override void Effect(ReagentEffectArgs args)
		{
			Solution solution = args.Source;
			if (solution == null || solution.Volume == 0)
			{
				return;
			}
			if (this._delta > 0f && solution.Temperature >= this._maxTemp)
			{
				return;
			}
			if (this._delta < 0f && solution.Temperature <= this._minTemp)
			{
				return;
			}
			float heatCap = solution.GetHeatCapacity(null);
			float deltaT = this._scaled ? (this._delta / heatCap * (float)args.Quantity) : (this._delta / heatCap);
			solution.Temperature = Math.Clamp(solution.Temperature + deltaT, this._minTemp, this._maxTemp);
		}

		// Token: 0x04001598 RID: 5528
		[DataField("delta", false, 1, true, false, null)]
		private float _delta;

		// Token: 0x04001599 RID: 5529
		[DataField("minTemp", false, 1, false, false, null)]
		private float _minTemp;

		// Token: 0x0400159A RID: 5530
		[DataField("maxTemp", false, 1, false, false, null)]
		private float _maxTemp = float.PositiveInfinity;

		// Token: 0x0400159B RID: 5531
		[DataField("scaled", false, 1, false, false, null)]
		private bool _scaled;
	}
}
