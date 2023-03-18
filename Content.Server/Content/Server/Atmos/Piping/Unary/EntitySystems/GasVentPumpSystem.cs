using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.MachineLinking.Events;
using Content.Server.MachineLinking.System;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Visuals;
using Content.Shared.Audio;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x0200074D RID: 1869
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasVentPumpSystem : EntitySystem
	{
		// Token: 0x06002744 RID: 10052 RVA: 0x000CEEA4 File Offset: 0x000CD0A4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasVentPumpComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasVentPumpComponent, AtmosDeviceUpdateEvent>(this.OnGasVentPumpUpdated), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasVentPumpComponent, AtmosDeviceDisabledEvent>(this.OnGasVentPumpLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, AtmosDeviceEnabledEvent>(new ComponentEventHandler<GasVentPumpComponent, AtmosDeviceEnabledEvent>(this.OnGasVentPumpEnterAtmosphere), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, AtmosAlarmEvent>(new ComponentEventHandler<GasVentPumpComponent, AtmosAlarmEvent>(this.OnAtmosAlarm), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, PowerChangedEvent>(new ComponentEventRefHandler<GasVentPumpComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<GasVentPumpComponent, DeviceNetworkPacketEvent>(this.OnPacketRecv), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, ComponentInit>(new ComponentEventHandler<GasVentPumpComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, ExaminedEvent>(new ComponentEventHandler<GasVentPumpComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, SignalReceivedEvent>(new ComponentEventHandler<GasVentPumpComponent, SignalReceivedEvent>(this.OnSignalReceived), null, null);
			base.SubscribeLocalEvent<GasVentPumpComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<GasVentPumpComponent, GasAnalyzerScanEvent>(this.OnAnalyzed), null, null);
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x000CEF80 File Offset: 0x000CD180
		private void OnGasVentPumpUpdated(EntityUid uid, GasVentPumpComponent vent, AtmosDeviceUpdateEvent args)
		{
			if (vent.Welded)
			{
				return;
			}
			VentPumpDirection pumpDirection = vent.PumpDirection;
			string text;
			if (pumpDirection != VentPumpDirection.Siphoning)
			{
				if (pumpDirection != VentPumpDirection.Releasing)
				{
					throw new ArgumentOutOfRangeException();
				}
				text = vent.Inlet;
			}
			else
			{
				text = vent.Outlet;
			}
			string nodeName = text;
			AtmosDeviceComponent device;
			NodeContainerComponent nodeContainer;
			PipeNode pipe;
			if (!vent.Enabled || !base.TryComp<AtmosDeviceComponent>(uid, ref device) || !base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(nodeName, out pipe))
			{
				return;
			}
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, true, true, null);
			if (environment == null)
			{
				return;
			}
			float pressureDelta = (float)(this._gameTiming.CurTime - device.LastProcess).TotalSeconds * vent.TargetPressureChange;
			if (vent.PumpDirection != VentPumpDirection.Releasing || pipe.Air.Pressure <= 0f)
			{
				if (vent.PumpDirection == VentPumpDirection.Siphoning && environment.Pressure > 0f)
				{
					if (pipe.Air.Pressure > vent.MaxPressure)
					{
						return;
					}
					if ((vent.PressureChecks & VentPressureBound.InternalBound) != VentPressureBound.NoBound)
					{
						pressureDelta = MathF.Min(pressureDelta, vent.InternalPressureBound - pipe.Air.Pressure);
					}
					if (pressureDelta <= 0f)
					{
						return;
					}
					float transferMoles = pressureDelta * pipe.Air.Volume / (environment.Temperature * 8.314463f);
					if ((vent.PressureChecks & VentPressureBound.ExternalBound) != VentPressureBound.NoBound)
					{
						float externalDelta = environment.Pressure - vent.ExternalPressureBound;
						if (externalDelta <= 0f)
						{
							return;
						}
						float maxTransfer = externalDelta * environment.Volume / (environment.Temperature * 8.314463f);
						transferMoles = MathF.Min(transferMoles, maxTransfer);
					}
					this._atmosphereSystem.Merge(pipe.Air, environment.Remove(transferMoles));
				}
				return;
			}
			if (environment.Pressure > vent.MaxPressure)
			{
				return;
			}
			if (environment.Pressure < vent.UnderPressureLockoutThreshold)
			{
				vent.UnderPressureLockout = true;
				return;
			}
			vent.UnderPressureLockout = false;
			if ((vent.PressureChecks & VentPressureBound.ExternalBound) != VentPressureBound.NoBound)
			{
				pressureDelta = MathF.Min(pressureDelta, vent.ExternalPressureBound - environment.Pressure);
			}
			if (pressureDelta <= 0f)
			{
				return;
			}
			float transferMoles2 = pressureDelta * environment.Volume / (pipe.Air.Temperature * 8.314463f);
			if ((vent.PressureChecks & VentPressureBound.InternalBound) != VentPressureBound.NoBound)
			{
				float internalDelta = pipe.Air.Pressure - vent.InternalPressureBound;
				if (internalDelta <= 0f)
				{
					return;
				}
				float maxTransfer2 = internalDelta * pipe.Air.Volume / (pipe.Air.Temperature * 8.314463f);
				transferMoles2 = MathF.Min(transferMoles2, maxTransfer2);
			}
			this._atmosphereSystem.Merge(environment, pipe.Air.Remove(transferMoles2));
		}

		// Token: 0x06002746 RID: 10054 RVA: 0x000CF215 File Offset: 0x000CD415
		private void OnGasVentPumpLeaveAtmosphere(EntityUid uid, GasVentPumpComponent component, AtmosDeviceDisabledEvent args)
		{
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x000CF220 File Offset: 0x000CD420
		private void OnGasVentPumpEnterAtmosphere(EntityUid uid, GasVentPumpComponent component, AtmosDeviceEnabledEvent args)
		{
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002748 RID: 10056 RVA: 0x000CF22B File Offset: 0x000CD42B
		private void OnAtmosAlarm(EntityUid uid, GasVentPumpComponent component, AtmosAlarmEvent args)
		{
			if (args.AlarmType == AtmosAlarmType.Danger)
			{
				component.Enabled = false;
			}
			else if (args.AlarmType == AtmosAlarmType.Normal)
			{
				component.Enabled = true;
			}
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002749 RID: 10057 RVA: 0x000CF258 File Offset: 0x000CD458
		private void OnPowerChanged(EntityUid uid, GasVentPumpComponent component, ref PowerChangedEvent args)
		{
			component.Enabled = args.Powered;
			this.UpdateState(uid, component, null);
		}

		// Token: 0x0600274A RID: 10058 RVA: 0x000CF270 File Offset: 0x000CD470
		private void OnPacketRecv(EntityUid uid, GasVentPumpComponent component, DeviceNetworkPacketEvent args)
		{
			DeviceNetworkComponent netConn;
			object cmd;
			if (!this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref netConn) || !args.Data.TryGetValue("command", out cmd))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload();
			string text = cmd as string;
			if (text != null)
			{
				if (text == "atmos_sync_data")
				{
					payload.Add("command", "atmos_sync_data");
					payload.Add("atmos_sync_data", component.ToAirAlarmData());
					DeviceNetworkSystem deviceNetSystem = this._deviceNetSystem;
					string senderAddress = args.SenderAddress;
					NetworkPayload data = payload;
					DeviceNetworkComponent device = netConn;
					deviceNetSystem.QueuePacket(uid, senderAddress, data, null, device);
					return;
				}
				if (!(text == "set_state"))
				{
					return;
				}
				GasVentPumpData setData;
				if (args.Data.TryGetValue<GasVentPumpData>("set_state", out setData))
				{
					component.FromAirAlarmData(setData);
					this.UpdateState(uid, component, null);
					return;
				}
			}
		}

		// Token: 0x0600274B RID: 10059 RVA: 0x000CF33E File Offset: 0x000CD53E
		private void OnInit(EntityUid uid, GasVentPumpComponent component, ComponentInit args)
		{
			if (component.CanLink)
			{
				this._signalSystem.EnsureReceiverPorts(uid, new string[]
				{
					component.PressurizePort,
					component.DepressurizePort
				});
			}
		}

		// Token: 0x0600274C RID: 10060 RVA: 0x000CF36C File Offset: 0x000CD56C
		private void OnSignalReceived(EntityUid uid, GasVentPumpComponent component, SignalReceivedEvent args)
		{
			if (!component.CanLink)
			{
				return;
			}
			if (args.Port == component.PressurizePort)
			{
				component.PumpDirection = VentPumpDirection.Releasing;
				component.ExternalPressureBound = component.PressurizePressure;
				component.PressureChecks = VentPressureBound.ExternalBound;
				this.UpdateState(uid, component, null);
				return;
			}
			if (args.Port == component.DepressurizePort)
			{
				component.PumpDirection = VentPumpDirection.Siphoning;
				component.ExternalPressureBound = component.DepressurizePressure;
				component.PressureChecks = VentPressureBound.ExternalBound;
				this.UpdateState(uid, component, null);
			}
		}

		// Token: 0x0600274D RID: 10061 RVA: 0x000CF3F0 File Offset: 0x000CD5F0
		private void UpdateState(EntityUid uid, GasVentPumpComponent vent, [Nullable(2)] AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				return;
			}
			this._ambientSoundSystem.SetAmbience(uid, true, null);
			if (!vent.Enabled)
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
				this._appearance.SetData(uid, VentPumpVisuals.State, VentPumpState.Off, appearance);
				return;
			}
			if (vent.PumpDirection == VentPumpDirection.Releasing)
			{
				this._appearance.SetData(uid, VentPumpVisuals.State, VentPumpState.Out, appearance);
				return;
			}
			if (vent.PumpDirection == VentPumpDirection.Siphoning)
			{
				this._appearance.SetData(uid, VentPumpVisuals.State, VentPumpState.In, appearance);
				return;
			}
			if (vent.Welded)
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
				this._appearance.SetData(uid, VentPumpVisuals.State, VentPumpState.Welded, appearance);
			}
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x000CF4BC File Offset: 0x000CD6BC
		private void OnExamine(EntityUid uid, GasVentPumpComponent component, ExaminedEvent args)
		{
			GasVentPumpComponent pumpComponent;
			if (!base.TryComp<GasVentPumpComponent>(uid, ref pumpComponent))
			{
				return;
			}
			if (args.IsInDetailsRange && pumpComponent.UnderPressureLockout)
			{
				args.PushMarkup(Loc.GetString("gas-vent-pump-uvlo"));
			}
		}

		// Token: 0x0600274F RID: 10063 RVA: 0x000CF4F8 File Offset: 0x000CD6F8
		private void OnAnalyzed(EntityUid uid, GasVentPumpComponent component, GasAnalyzerScanEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			Dictionary<string, GasMixture> gasMixDict = new Dictionary<string, GasMixture>();
			VentPumpDirection pumpDirection = component.PumpDirection;
			string text;
			if (pumpDirection != VentPumpDirection.Siphoning)
			{
				if (pumpDirection != VentPumpDirection.Releasing)
				{
					throw new ArgumentOutOfRangeException();
				}
				text = component.Inlet;
			}
			else
			{
				text = component.Outlet;
			}
			string nodeName = text;
			PipeNode pipe;
			if (nodeContainer.TryGetNode<PipeNode>(nodeName, out pipe))
			{
				gasMixDict.Add(nodeName, pipe.Air);
			}
			args.GasMixtures = gasMixDict;
		}

		// Token: 0x0400186F RID: 6255
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001870 RID: 6256
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetSystem;

		// Token: 0x04001871 RID: 6257
		[Dependency]
		private readonly SignalLinkerSystem _signalSystem;

		// Token: 0x04001872 RID: 6258
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001873 RID: 6259
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04001874 RID: 6260
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
