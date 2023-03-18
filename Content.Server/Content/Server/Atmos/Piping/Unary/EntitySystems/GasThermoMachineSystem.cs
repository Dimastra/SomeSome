using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.Construction;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping;
using Content.Shared.Atmos.Piping.Unary.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x0200074C RID: 1868
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasThermoMachineSystem : EntitySystem
	{
		// Token: 0x0600273B RID: 10043 RVA: 0x000CEB64 File Offset: 0x000CCD64
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasThermoMachineComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasThermoMachineComponent, AtmosDeviceUpdateEvent>(this.OnThermoMachineUpdated), null, null);
			base.SubscribeLocalEvent<GasThermoMachineComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasThermoMachineComponent, AtmosDeviceDisabledEvent>(this.OnThermoMachineLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasThermoMachineComponent, RefreshPartsEvent>(new ComponentEventHandler<GasThermoMachineComponent, RefreshPartsEvent>(this.OnGasThermoRefreshParts), null, null);
			base.SubscribeLocalEvent<GasThermoMachineComponent, UpgradeExamineEvent>(new ComponentEventHandler<GasThermoMachineComponent, UpgradeExamineEvent>(this.OnGasThermoUpgradeExamine), null, null);
			base.SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineToggleMessage>(new ComponentEventHandler<GasThermoMachineComponent, GasThermomachineToggleMessage>(this.OnToggleMessage), null, null);
			base.SubscribeLocalEvent<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>(new ComponentEventHandler<GasThermoMachineComponent, GasThermomachineChangeTemperatureMessage>(this.OnChangeTemperature), null, null);
		}

		// Token: 0x0600273C RID: 10044 RVA: 0x000CEBF0 File Offset: 0x000CCDF0
		private void OnThermoMachineUpdated(EntityUid uid, GasThermoMachineComponent thermoMachine, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			PipeNode inlet;
			if (!thermoMachine.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(thermoMachine.InletName, out inlet))
			{
				this.DirtyUI(uid, thermoMachine, null);
				this._appearance.SetData(uid, ThermoMachineVisuals.Enabled, false, null);
				return;
			}
			float airHeatCapacity = this._atmosphereSystem.GetHeatCapacity(inlet.Air);
			float combinedHeatCapacity = airHeatCapacity + thermoMachine.HeatCapacity;
			if (!MathHelper.CloseTo(combinedHeatCapacity, 0f, 0.001f))
			{
				this._appearance.SetData(uid, ThermoMachineVisuals.Enabled, true, null);
				float combinedEnergy = thermoMachine.HeatCapacity * thermoMachine.TargetTemperature + airHeatCapacity * inlet.Air.Temperature;
				inlet.Air.Temperature = combinedEnergy / combinedHeatCapacity;
			}
		}

		// Token: 0x0600273D RID: 10045 RVA: 0x000CECBA File Offset: 0x000CCEBA
		private void OnThermoMachineLeaveAtmosphere(EntityUid uid, GasThermoMachineComponent component, AtmosDeviceDisabledEvent args)
		{
			this._appearance.SetData(uid, ThermoMachineVisuals.Enabled, false, null);
			this.DirtyUI(uid, component, null);
		}

		// Token: 0x0600273E RID: 10046 RVA: 0x000CECE0 File Offset: 0x000CCEE0
		private void OnGasThermoRefreshParts(EntityUid uid, GasThermoMachineComponent component, RefreshPartsEvent args)
		{
			float matterBinRating = args.PartRatings[component.MachinePartHeatCapacity];
			float laserRating = args.PartRatings[component.MachinePartTemperature];
			component.HeatCapacity = component.BaseHeatCapacity * MathF.Pow(matterBinRating, 2f);
			ThermoMachineMode mode = component.Mode;
			if (mode != ThermoMachineMode.Freezer)
			{
				if (mode == ThermoMachineMode.Heater)
				{
					component.MaxTemperature = component.BaseMaxTemperature + component.MaxTemperatureDelta * laserRating;
					component.MinTemperature = 293.15f;
				}
			}
			else
			{
				component.MinTemperature = MathF.Max(component.BaseMinTemperature - component.MinTemperatureDelta * laserRating, 2.7f);
				component.MaxTemperature = 293.15f;
			}
			this.DirtyUI(uid, component, null);
		}

		// Token: 0x0600273F RID: 10047 RVA: 0x000CED8C File Offset: 0x000CCF8C
		private void OnGasThermoUpgradeExamine(EntityUid uid, GasThermoMachineComponent component, UpgradeExamineEvent args)
		{
			ThermoMachineMode mode = component.Mode;
			if (mode != ThermoMachineMode.Freezer)
			{
				if (mode == ThermoMachineMode.Heater)
				{
					args.AddPercentageUpgrade("gas-thermo-component-upgrade-heating", component.MaxTemperature / (component.BaseMaxTemperature + component.MaxTemperatureDelta));
				}
			}
			else
			{
				args.AddPercentageUpgrade("gas-thermo-component-upgrade-cooling", component.MinTemperature / (component.BaseMinTemperature - component.MinTemperatureDelta));
			}
			args.AddPercentageUpgrade("gas-thermo-component-upgrade-heat-capacity", component.HeatCapacity / component.BaseHeatCapacity);
		}

		// Token: 0x06002740 RID: 10048 RVA: 0x000CEDFF File Offset: 0x000CCFFF
		private void OnToggleMessage(EntityUid uid, GasThermoMachineComponent component, GasThermomachineToggleMessage args)
		{
			component.Enabled = !component.Enabled;
			this.DirtyUI(uid, component, null);
		}

		// Token: 0x06002741 RID: 10049 RVA: 0x000CEE19 File Offset: 0x000CD019
		private void OnChangeTemperature(EntityUid uid, GasThermoMachineComponent component, GasThermomachineChangeTemperatureMessage args)
		{
			component.TargetTemperature = Math.Clamp(args.Temperature, component.MinTemperature, component.MaxTemperature);
			this.DirtyUI(uid, component, null);
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x000CEE44 File Offset: 0x000CD044
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasThermoMachineComponent thermo, ServerUserInterfaceComponent ui = null)
		{
			if (!base.Resolve<GasThermoMachineComponent, ServerUserInterfaceComponent>(uid, ref thermo, ref ui, false))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, ThermomachineUiKey.Key, new GasThermomachineBoundUserInterfaceState(thermo.MinTemperature, thermo.MaxTemperature, thermo.TargetTemperature, thermo.Enabled, thermo.Mode), null, ui, true);
		}

		// Token: 0x0400186C RID: 6252
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x0400186D RID: 6253
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x0400186E RID: 6254
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;
	}
}
