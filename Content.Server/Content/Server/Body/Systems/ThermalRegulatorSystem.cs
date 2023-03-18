using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Components;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.ActionBlocker;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Body.Systems
{
	// Token: 0x0200070E RID: 1806
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThermalRegulatorSystem : EntitySystem
	{
		// Token: 0x06002613 RID: 9747 RVA: 0x000C9390 File Offset: 0x000C7590
		public override void Update(float frameTime)
		{
			foreach (ThermalRegulatorComponent regulator in this.EntityManager.EntityQuery<ThermalRegulatorComponent>(false))
			{
				regulator.AccumulatedFrametime += frameTime;
				if (regulator.AccumulatedFrametime >= 1f)
				{
					regulator.AccumulatedFrametime -= 1f;
					this.ProcessThermalRegulation(regulator.Owner, regulator);
				}
			}
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x000C9418 File Offset: 0x000C7618
		private void ProcessThermalRegulation(EntityUid uid, ThermalRegulatorComponent comp)
		{
			TemperatureComponent temperatureComponent;
			if (!this.EntityManager.TryGetComponent<TemperatureComponent>(uid, ref temperatureComponent))
			{
				return;
			}
			float totalMetabolismTempChange = comp.MetabolismHeat - comp.RadiatedHeat;
			float targetHeat = Math.Abs(temperatureComponent.CurrentTemperature - comp.NormalBodyTemperature) * temperatureComponent.HeatCapacity;
			if (temperatureComponent.CurrentTemperature > comp.NormalBodyTemperature)
			{
				totalMetabolismTempChange -= Math.Min(targetHeat, comp.ImplicitHeatRegulation);
			}
			else
			{
				totalMetabolismTempChange += Math.Min(targetHeat, comp.ImplicitHeatRegulation);
			}
			this._tempSys.ChangeHeat(uid, totalMetabolismTempChange, true, temperatureComponent);
			float num = Math.Abs(temperatureComponent.CurrentTemperature - comp.NormalBodyTemperature);
			targetHeat = num * temperatureComponent.HeatCapacity;
			if (num > comp.ThermalRegulationTemperatureThreshold)
			{
				return;
			}
			if (temperatureComponent.CurrentTemperature > comp.NormalBodyTemperature)
			{
				if (!this._actionBlockerSys.CanSweat(uid))
				{
					return;
				}
				this._tempSys.ChangeHeat(uid, -Math.Min(targetHeat, comp.SweatHeatRegulation), true, temperatureComponent);
				return;
			}
			else
			{
				if (!this._actionBlockerSys.CanShiver(uid))
				{
					return;
				}
				this._tempSys.ChangeHeat(uid, Math.Min(targetHeat, comp.ShiveringHeatRegulation), true, temperatureComponent);
				return;
			}
		}

		// Token: 0x04001785 RID: 6021
		[Dependency]
		private readonly TemperatureSystem _tempSys;

		// Token: 0x04001786 RID: 6022
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSys;
	}
}
