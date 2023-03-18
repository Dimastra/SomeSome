using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.EntitySystems;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x02000782 RID: 1922
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosMonitorSystem : EntitySystem
	{
		// Token: 0x060028E1 RID: 10465 RVA: 0x000D50C0 File Offset: 0x000D32C0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AtmosMonitorComponent, ComponentInit>(new ComponentEventHandler<AtmosMonitorComponent, ComponentInit>(this.OnAtmosMonitorInit), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, ComponentStartup>(new ComponentEventHandler<AtmosMonitorComponent, ComponentStartup>(this.OnAtmosMonitorStartup), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<AtmosMonitorComponent, AtmosDeviceUpdateEvent>(this.OnAtmosUpdate), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, TileFireEvent>(new ComponentEventRefHandler<AtmosMonitorComponent, TileFireEvent>(this.OnFireEvent), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, PowerChangedEvent>(new ComponentEventRefHandler<AtmosMonitorComponent, PowerChangedEvent>(this.OnPowerChangedEvent), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, BeforePacketSentEvent>(new ComponentEventHandler<AtmosMonitorComponent, BeforePacketSentEvent>(this.BeforePacketRecv), null, null);
			base.SubscribeLocalEvent<AtmosMonitorComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<AtmosMonitorComponent, DeviceNetworkPacketEvent>(this.OnPacketRecv), null, null);
		}

		// Token: 0x060028E2 RID: 10466 RVA: 0x000D515C File Offset: 0x000D335C
		private void OnAtmosMonitorInit(EntityUid uid, AtmosMonitorComponent component, ComponentInit args)
		{
			if (component.TemperatureThresholdId != null)
			{
				component.TemperatureThreshold = new AtmosAlarmThreshold(this._prototypeManager.Index<AtmosAlarmThreshold>(component.TemperatureThresholdId));
			}
			if (component.PressureThresholdId != null)
			{
				component.PressureThreshold = new AtmosAlarmThreshold(this._prototypeManager.Index<AtmosAlarmThreshold>(component.PressureThresholdId));
			}
			if (component.GasThresholdIds != null)
			{
				component.GasThresholds = new Dictionary<Gas, AtmosAlarmThreshold>();
				foreach (KeyValuePair<Gas, string> keyValuePair in component.GasThresholdIds)
				{
					Gas gas2;
					string text;
					keyValuePair.Deconstruct(out gas2, out text);
					Gas gas = gas2;
					string id = text;
					AtmosAlarmThreshold gasThreshold;
					if (this._prototypeManager.TryIndex<AtmosAlarmThreshold>(id, ref gasThreshold))
					{
						component.GasThresholds.Add(gas, new AtmosAlarmThreshold(gasThreshold));
					}
				}
			}
		}

		// Token: 0x060028E3 RID: 10467 RVA: 0x000D5238 File Offset: 0x000D3438
		private void OnAtmosMonitorStartup(EntityUid uid, AtmosMonitorComponent component, ComponentStartup args)
		{
			AtmosDeviceComponent atmosDeviceComponent;
			if (!base.HasComp<ApcPowerReceiverComponent>(uid) && base.TryComp<AtmosDeviceComponent>(uid, ref atmosDeviceComponent))
			{
				this._atmosDeviceSystem.LeaveAtmosphere(atmosDeviceComponent);
			}
		}

		// Token: 0x060028E4 RID: 10468 RVA: 0x000D5265 File Offset: 0x000D3465
		private void BeforePacketRecv(EntityUid uid, AtmosMonitorComponent component, BeforePacketSentEvent args)
		{
			if (!component.NetEnabled)
			{
				args.Cancel();
			}
		}

		// Token: 0x060028E5 RID: 10469 RVA: 0x000D5278 File Offset: 0x000D3478
		private void OnPacketRecv(EntityUid uid, AtmosMonitorComponent component, DeviceNetworkPacketEvent args)
		{
			string cmd;
			if (!args.Data.TryGetValue<string>("command", out cmd))
			{
				return;
			}
			if (cmd == "atmos_register_device")
			{
				component.RegisteredDevices.Add(args.SenderAddress);
				return;
			}
			if (cmd == "atmos_deregister_device")
			{
				component.RegisteredDevices.Remove(args.SenderAddress);
				return;
			}
			if (!(cmd == "atmos_alarmable_reset_all"))
			{
				AtmosAlarmThreshold thresholdData;
				AtmosMonitorThresholdType? thresholdType;
				if (!(cmd == "atmos_monitor_set_threshold"))
				{
					if (!(cmd == "atmos_sync_data"))
					{
						return;
					}
					NetworkPayload payload = new NetworkPayload();
					payload.Add("command", "atmos_sync_data");
					if (component.TileGas != null)
					{
						Dictionary<Gas, float> gases = new Dictionary<Gas, float>();
						foreach (Gas gas in Enum.GetValues<Gas>())
						{
							gases.Add(gas, component.TileGas.GetMoles(gas));
						}
						payload.Add("atmos_sync_data", new AtmosSensorData(component.TileGas.Pressure, component.TileGas.Temperature, component.TileGas.TotalMoles, component.LastAlarmState, gases, component.PressureThreshold ?? new AtmosAlarmThreshold(), component.TemperatureThreshold ?? new AtmosAlarmThreshold(), component.GasThresholds ?? new Dictionary<Gas, AtmosAlarmThreshold>()));
					}
					this._deviceNetSystem.QueuePacket(uid, args.SenderAddress, payload, null, null);
				}
				else if (args.Data.TryGetValue<AtmosAlarmThreshold>("atmos_monitor_threshold_data", out thresholdData) && args.Data.TryGetValue<AtmosMonitorThresholdType?>("atmos_monitor_threshold_type", out thresholdType))
				{
					Gas? gas2;
					args.Data.TryGetValue<Gas?>("atmos_monitor_threshold_gas", out gas2);
					this.SetThreshold(uid, thresholdType.Value, thresholdData, gas2, null);
					return;
				}
				return;
			}
			this.Reset(uid);
		}

		// Token: 0x060028E6 RID: 10470 RVA: 0x000D5448 File Offset: 0x000D3648
		private void OnPowerChangedEvent(EntityUid uid, AtmosMonitorComponent component, ref PowerChangedEvent args)
		{
			AtmosDeviceComponent atmosDeviceComponent;
			if (base.TryComp<AtmosDeviceComponent>(uid, ref atmosDeviceComponent))
			{
				if (!args.Powered)
				{
					if (atmosDeviceComponent.JoinedGrid != null)
					{
						this._atmosDeviceSystem.LeaveAtmosphere(atmosDeviceComponent);
						component.TileGas = null;
						return;
					}
				}
				else if (args.Powered)
				{
					if (atmosDeviceComponent.JoinedGrid == null)
					{
						this._atmosDeviceSystem.JoinAtmosphere(atmosDeviceComponent);
						GasMixture air = this._atmosphereSystem.GetContainingMixture(uid, true, false, null);
						component.TileGas = air;
					}
					this.Alert(uid, component.LastAlarmState, null, null);
				}
			}
		}

		// Token: 0x060028E7 RID: 10471 RVA: 0x000D54D8 File Offset: 0x000D36D8
		private void OnFireEvent(EntityUid uid, AtmosMonitorComponent component, ref TileFireEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			if (component.MonitorFire && component.LastAlarmState != AtmosAlarmType.Danger)
			{
				component.TrippedThresholds.Add(AtmosMonitorThresholdType.Temperature);
				this.Alert(uid, AtmosAlarmType.Danger, null, component);
			}
			AtmosAlarmType temperatureState;
			if (component.TemperatureThreshold != null && component.TemperatureThreshold.CheckThreshold(args.Temperature, out temperatureState) && temperatureState > component.LastAlarmState)
			{
				component.TrippedThresholds.Add(AtmosMonitorThresholdType.Temperature);
				this.Alert(uid, AtmosAlarmType.Danger, null, component);
			}
		}

		// Token: 0x060028E8 RID: 10472 RVA: 0x000D555C File Offset: 0x000D375C
		private void OnAtmosUpdate(EntityUid uid, AtmosMonitorComponent component, AtmosDeviceUpdateEvent args)
		{
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			AtmosDeviceComponent atmosDeviceComponent;
			if (!base.TryComp<AtmosDeviceComponent>(uid, ref atmosDeviceComponent) || atmosDeviceComponent.JoinedGrid == null)
			{
				return;
			}
			if (component.TemperatureThreshold == null && component.PressureThreshold == null && component.GasThresholds == null)
			{
				return;
			}
			this.UpdateState(uid, component.TileGas, component);
		}

		// Token: 0x060028E9 RID: 10473 RVA: 0x000D55C0 File Offset: 0x000D37C0
		[NullableContext(2)]
		private void UpdateState(EntityUid uid, GasMixture air, AtmosMonitorComponent monitor = null)
		{
			if (air == null)
			{
				return;
			}
			if (!base.Resolve<AtmosMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			AtmosAlarmType state = AtmosAlarmType.Normal;
			HashSet<AtmosMonitorThresholdType> alarmTypes = new HashSet<AtmosMonitorThresholdType>(monitor.TrippedThresholds);
			AtmosAlarmType temperatureState;
			if (monitor.TemperatureThreshold != null && monitor.TemperatureThreshold.CheckThreshold(air.Temperature, out temperatureState))
			{
				if (temperatureState > state)
				{
					state = temperatureState;
					alarmTypes.Add(AtmosMonitorThresholdType.Temperature);
				}
				else if (temperatureState == AtmosAlarmType.Normal)
				{
					alarmTypes.Remove(AtmosMonitorThresholdType.Temperature);
				}
			}
			AtmosAlarmType pressureState;
			if (monitor.PressureThreshold != null && monitor.PressureThreshold.CheckThreshold(air.Pressure, out pressureState))
			{
				if (pressureState > state)
				{
					state = pressureState;
					alarmTypes.Add(AtmosMonitorThresholdType.Pressure);
				}
				else if (pressureState == AtmosAlarmType.Normal)
				{
					alarmTypes.Remove(AtmosMonitorThresholdType.Pressure);
				}
			}
			if (monitor.GasThresholds != null)
			{
				bool tripped = false;
				foreach (KeyValuePair<Gas, AtmosAlarmThreshold> keyValuePair in monitor.GasThresholds)
				{
					Gas gas2;
					AtmosAlarmThreshold atmosAlarmThreshold;
					keyValuePair.Deconstruct(out gas2, out atmosAlarmThreshold);
					Gas gas = gas2;
					AtmosAlarmThreshold atmosAlarmThreshold2 = atmosAlarmThreshold;
					float gasRatio = air.GetMoles(gas) / air.TotalMoles;
					AtmosAlarmType gasState;
					if (atmosAlarmThreshold2.CheckThreshold(gasRatio, out gasState) && gasState > state)
					{
						state = gasState;
						tripped = true;
					}
				}
				if (tripped)
				{
					alarmTypes.Add(AtmosMonitorThresholdType.Gas);
				}
				else
				{
					alarmTypes.Remove(AtmosMonitorThresholdType.Gas);
				}
			}
			if (state != monitor.LastAlarmState || !alarmTypes.SetEquals(monitor.TrippedThresholds))
			{
				this.Alert(uid, state, alarmTypes, monitor);
			}
		}

		// Token: 0x060028EA RID: 10474 RVA: 0x000D571C File Offset: 0x000D391C
		[NullableContext(2)]
		public void Alert(EntityUid uid, AtmosAlarmType state, HashSet<AtmosMonitorThresholdType> alarms = null, AtmosMonitorComponent monitor = null)
		{
			if (!base.Resolve<AtmosMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			monitor.LastAlarmState = state;
			monitor.TrippedThresholds = (alarms ?? monitor.TrippedThresholds);
			this.BroadcastAlertPacket(monitor, null);
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x000D574F File Offset: 0x000D394F
		private void Reset(EntityUid uid)
		{
			this.Alert(uid, AtmosAlarmType.Normal, null, null);
		}

		// Token: 0x060028EC RID: 10476 RVA: 0x000D575C File Offset: 0x000D395C
		private void BroadcastAlertPacket(AtmosMonitorComponent monitor, [Nullable(2)] TagComponent tags = null)
		{
			if (!monitor.NetEnabled)
			{
				return;
			}
			if (!base.Resolve<TagComponent>(monitor.Owner, ref tags, false))
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_alarm";
			networkPayload["set_state"] = monitor.LastAlarmState;
			networkPayload["atmos_alarm_source"] = tags.Tags;
			networkPayload["atmos_alarm_types"] = monitor.TrippedThresholds;
			NetworkPayload payload = networkPayload;
			foreach (string addr in monitor.RegisteredDevices)
			{
				this._deviceNetSystem.QueuePacket(monitor.Owner, addr, payload, null, null);
			}
		}

		// Token: 0x060028ED RID: 10477 RVA: 0x000D5830 File Offset: 0x000D3A30
		public void SetThreshold(EntityUid uid, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null, [Nullable(2)] AtmosMonitorComponent monitor = null)
		{
			if (!base.Resolve<AtmosMonitorComponent>(uid, ref monitor, true))
			{
				return;
			}
			switch (type)
			{
			case AtmosMonitorThresholdType.Temperature:
				monitor.TemperatureThreshold = threshold;
				return;
			case AtmosMonitorThresholdType.Pressure:
				monitor.PressureThreshold = threshold;
				return;
			case AtmosMonitorThresholdType.Gas:
				if (gas == null || monitor.GasThresholds == null)
				{
					return;
				}
				monitor.GasThresholds[gas.Value] = threshold;
				return;
			default:
				return;
			}
		}

		// Token: 0x04001957 RID: 6487
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001958 RID: 6488
		[Dependency]
		private readonly AtmosDeviceSystem _atmosDeviceSystem;

		// Token: 0x04001959 RID: 6489
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetSystem;

		// Token: 0x0400195A RID: 6490
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400195B RID: 6491
		public const string AtmosMonitorSetThresholdCmd = "atmos_monitor_set_threshold";

		// Token: 0x0400195C RID: 6492
		public const string AtmosMonitorThresholdData = "atmos_monitor_threshold_data";

		// Token: 0x0400195D RID: 6493
		public const string AtmosMonitorThresholdDataType = "atmos_monitor_threshold_type";

		// Token: 0x0400195E RID: 6494
		public const string AtmosMonitorThresholdGasType = "atmos_monitor_threshold_gas";
	}
}
