using System;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.ReagentEffects
{
	// Token: 0x02000658 RID: 1624
	public sealed class AdjustTemperature : ReagentEffect
	{
		// Token: 0x0600224E RID: 8782 RVA: 0x000B39BC File Offset: 0x000B1BBC
		public override void Effect(ReagentEffectArgs args)
		{
			TemperatureComponent temp;
			if (args.EntityManager.TryGetComponent<TemperatureComponent>(args.SolutionEntity, ref temp))
			{
				TemperatureSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<TemperatureSystem>();
				float amount = this.Amount;
				amount *= args.Scale;
				entitySystem.ChangeHeat(args.SolutionEntity, amount, true, temp);
			}
		}

		// Token: 0x04001538 RID: 5432
		[DataField("amount", false, 1, false, false, null)]
		public float Amount;
	}
}
