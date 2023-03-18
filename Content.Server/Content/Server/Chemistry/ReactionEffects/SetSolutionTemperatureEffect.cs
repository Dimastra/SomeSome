using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReactionEffects
{
	// Token: 0x02000690 RID: 1680
	[DataDefinition]
	public sealed class SetSolutionTemperatureEffect : ReagentEffect
	{
		// Token: 0x060022D5 RID: 8917 RVA: 0x000B5338 File Offset: 0x000B3538
		public override void Effect(ReagentEffectArgs args)
		{
			Solution solution = args.Source;
			if (solution == null)
			{
				return;
			}
			solution.Temperature = this._temperature;
		}

		// Token: 0x04001593 RID: 5523
		[DataField("temperature", false, 1, true, false, null)]
		private float _temperature;
	}
}
