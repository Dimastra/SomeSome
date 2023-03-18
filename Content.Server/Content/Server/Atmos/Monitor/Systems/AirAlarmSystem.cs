using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Wires;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.DeviceNetwork;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x0200077E RID: 1918
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AirAlarmSystem : EntitySystem
	{
		// Token: 0x060028AD RID: 10413 RVA: 0x000D3D2A File Offset: 0x000D1F2A
		public void SetData(EntityUid uid, string address, IAtmosDeviceData data)
		{
			this._atmosDevNetSystem.SetDeviceState(uid, address, data);
			this._atmosDevNetSystem.Sync(uid, address);
		}

		// Token: 0x060028AE RID: 10414 RVA: 0x000D3D47 File Offset: 0x000D1F47
		private void SyncAllDevices(EntityUid uid)
		{
			this._atmosDevNetSystem.Sync(uid, null);
		}

		// Token: 0x060028AF RID: 10415 RVA: 0x000D3D56 File Offset: 0x000D1F56
		private void SyncDevice(EntityUid uid, string address)
		{
			this._atmosDevNetSystem.Sync(uid, address);
		}

		// Token: 0x060028B0 RID: 10416 RVA: 0x000D3D65 File Offset: 0x000D1F65
		private void SyncRegisterAllDevices(EntityUid uid)
		{
			this._atmosDevNetSystem.Register(uid, null);
			this._atmosDevNetSystem.Sync(uid, null);
		}

		// Token: 0x060028B1 RID: 10417 RVA: 0x000D3D84 File Offset: 0x000D1F84
		[NullableContext(2)]
		private void SyncAllSensors(EntityUid uid, AirAlarmComponent monitor = null)
		{
			if (!base.Resolve<AirAlarmComponent>(uid, ref monitor, true))
			{
				return;
			}
			foreach (string addr in monitor.SensorData.Keys)
			{
				this.SyncDevice(uid, addr);
			}
		}

		// Token: 0x060028B2 RID: 10418 RVA: 0x000D3DEC File Offset: 0x000D1FEC
		private void SetThreshold(EntityUid uid, string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_monitor_set_threshold";
			networkPayload["atmos_monitor_threshold_type"] = type;
			networkPayload["atmos_monitor_threshold_data"] = threshold;
			NetworkPayload payload = networkPayload;
			if (gas != null)
			{
				payload.Add("atmos_monitor_threshold_gas", gas);
			}
			this._deviceNet.QueuePacket(uid, address, payload, null, null);
			this.SyncDevice(uid, address);
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x000D3E6C File Offset: 0x000D206C
		private void SyncMode(EntityUid uid, AirAlarmMode mode)
		{
			AtmosMonitorComponent monitor;
			if (this.EntityManager.TryGetComponent<AtmosMonitorComponent>(uid, ref monitor) && !monitor.NetEnabled)
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "air_alarm_set_mode";
			networkPayload["air_alarm_set_mode"] = mode;
			NetworkPayload payload = networkPayload;
			this._deviceNet.QueuePacket(uid, null, payload, null, null);
		}

		// Token: 0x060028B4 RID: 10420 RVA: 0x000D3ED4 File Offset: 0x000D20D4
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AirAlarmComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<AirAlarmComponent, DeviceNetworkPacketEvent>(this.OnPacketRecv), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<AirAlarmComponent, AtmosDeviceUpdateEvent>(this.OnAtmosUpdate), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AtmosAlarmEvent>(new ComponentEventHandler<AirAlarmComponent, AtmosAlarmEvent>(this.OnAtmosAlarm), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, PowerChangedEvent>(new ComponentEventRefHandler<AirAlarmComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AirAlarmResyncAllDevicesMessage>(new ComponentEventHandler<AirAlarmComponent, AirAlarmResyncAllDevicesMessage>(this.OnResyncAll), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AirAlarmUpdateAlarmModeMessage>(new ComponentEventHandler<AirAlarmComponent, AirAlarmUpdateAlarmModeMessage>(this.OnUpdateAlarmMode), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AirAlarmUpdateAlarmThresholdMessage>(new ComponentEventHandler<AirAlarmComponent, AirAlarmUpdateAlarmThresholdMessage>(this.OnUpdateThreshold), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AirAlarmUpdateDeviceDataMessage>(new ComponentEventHandler<AirAlarmComponent, AirAlarmUpdateDeviceDataMessage>(this.OnUpdateDeviceData), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, AirAlarmTabSetMessage>(new ComponentEventHandler<AirAlarmComponent, AirAlarmTabSetMessage>(this.OnTabChange), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, DeviceListUpdateEvent>(new ComponentEventHandler<AirAlarmComponent, DeviceListUpdateEvent>(this.OnDeviceListUpdate), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, BoundUIClosedEvent>(new ComponentEventHandler<AirAlarmComponent, BoundUIClosedEvent>(this.OnClose), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, ComponentShutdown>(new ComponentEventHandler<AirAlarmComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<AirAlarmComponent, InteractHandEvent>(new ComponentEventHandler<AirAlarmComponent, InteractHandEvent>(this.OnInteract), null, null);
		}

		// Token: 0x060028B5 RID: 10421 RVA: 0x000D3FE8 File Offset: 0x000D21E8
		private void OnDeviceListUpdate(EntityUid uid, AirAlarmComponent component, DeviceListUpdateEvent args)
		{
			EntityQuery<DeviceNetworkComponent> query = base.GetEntityQuery<DeviceNetworkComponent>();
			foreach (EntityUid device in args.OldDevices)
			{
				DeviceNetworkComponent deviceNet;
				if (query.TryGetComponent(device, ref deviceNet))
				{
					this._atmosDevNetSystem.Deregister(uid, deviceNet.Address);
				}
			}
			component.ScrubberData.Clear();
			component.SensorData.Clear();
			component.VentData.Clear();
			component.KnownDevices.Clear();
			this.UpdateUI(uid, component, null, null);
			this.SyncRegisterAllDevices(uid);
		}

		// Token: 0x060028B6 RID: 10422 RVA: 0x000D4098 File Offset: 0x000D2298
		private void OnTabChange(EntityUid uid, AirAlarmComponent component, AirAlarmTabSetMessage msg)
		{
			component.CurrentTab = msg.Tab;
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028B7 RID: 10423 RVA: 0x000D40B0 File Offset: 0x000D22B0
		private void OnPowerChanged(EntityUid uid, AirAlarmComponent component, ref PowerChangedEvent args)
		{
			if (args.Powered)
			{
				return;
			}
			this.ForceCloseAllInterfaces(uid);
			component.CurrentModeUpdater = null;
			component.KnownDevices.Clear();
			component.ScrubberData.Clear();
			component.SensorData.Clear();
			component.VentData.Clear();
		}

		// Token: 0x060028B8 RID: 10424 RVA: 0x000D4100 File Offset: 0x000D2300
		private void OnClose(EntityUid uid, AirAlarmComponent component, BoundUIClosedEvent args)
		{
			component.ActivePlayers.Remove(args.Session.UserId);
			if (component.ActivePlayers.Count == 0)
			{
				this.RemoveActiveInterface(uid);
			}
		}

		// Token: 0x060028B9 RID: 10425 RVA: 0x000D412D File Offset: 0x000D232D
		private void OnShutdown(EntityUid uid, AirAlarmComponent component, ComponentShutdown args)
		{
			this._activeUserInterfaces.Remove(uid);
		}

		// Token: 0x060028BA RID: 10426 RVA: 0x000D413C File Offset: 0x000D233C
		private void OnInteract(EntityUid uid, AirAlarmComponent component, InteractHandEvent args)
		{
			if (!this._interactionSystem.InRangeUnobstructed(args.User, args.Target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return;
			}
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			WiresComponent wire;
			if (this.EntityManager.TryGetComponent<WiresComponent>(uid, ref wire) && wire.IsPanelOpen)
			{
				args.Handled = false;
				return;
			}
			if (!this.IsPowered(uid, this.EntityManager, null))
			{
				return;
			}
			BoundUserInterface uiOrNull = this._uiSystem.GetUiOrNull(component.Owner, SharedAirAlarmInterfaceKey.Key, null);
			if (uiOrNull != null)
			{
				uiOrNull.Open(actor.PlayerSession);
			}
			component.ActivePlayers.Add(actor.PlayerSession.UserId);
			this.AddActiveInterface(uid);
			this.SyncAllDevices(uid);
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028BB RID: 10427 RVA: 0x000D4210 File Offset: 0x000D2410
		private void OnResyncAll(EntityUid uid, AirAlarmComponent component, AirAlarmResyncAllDevicesMessage args)
		{
			if (!this.AccessCheck(uid, args.Session.AttachedEntity, component))
			{
				return;
			}
			component.KnownDevices.Clear();
			component.VentData.Clear();
			component.ScrubberData.Clear();
			component.SensorData.Clear();
			this.SyncRegisterAllDevices(uid);
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x000D4268 File Offset: 0x000D2468
		private void OnUpdateAlarmMode(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateAlarmModeMessage args)
		{
			string addr = string.Empty;
			DeviceNetworkComponent netConn;
			if (this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref netConn))
			{
				addr = netConn.Address;
			}
			if (this.AccessCheck(uid, args.Session.AttachedEntity, component))
			{
				this.SetMode(uid, addr, args.Mode, false, null);
				return;
			}
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x000D42C4 File Offset: 0x000D24C4
		private void OnUpdateThreshold(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateAlarmThresholdMessage args)
		{
			if (this.AccessCheck(uid, args.Session.AttachedEntity, component))
			{
				this.SetThreshold(uid, args.Address, args.Type, args.Threshold, args.Gas);
				return;
			}
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028BE RID: 10430 RVA: 0x000D4310 File Offset: 0x000D2510
		private void OnUpdateDeviceData(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateDeviceDataMessage args)
		{
			if (this.AccessCheck(uid, args.Session.AttachedEntity, component))
			{
				this.SetDeviceData(uid, args.Address, args.Data, null);
				return;
			}
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028BF RID: 10431 RVA: 0x000D4348 File Offset: 0x000D2548
		[NullableContext(2)]
		private bool AccessCheck(EntityUid uid, EntityUid? user, AirAlarmComponent component = null)
		{
			if (!base.Resolve<AirAlarmComponent>(uid, ref component, true))
			{
				return false;
			}
			AccessReaderComponent reader;
			if (!this.EntityManager.TryGetComponent<AccessReaderComponent>(uid, ref reader) || user == null)
			{
				return false;
			}
			if (!this._accessSystem.IsAllowed(user.Value, reader))
			{
				this._popup.PopupEntity(Loc.GetString("air-alarm-ui-access-denied"), user.Value, user.Value, PopupType.Small);
				return false;
			}
			return true;
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x000D43BC File Offset: 0x000D25BC
		private void OnAtmosAlarm(EntityUid uid, AirAlarmComponent component, AtmosAlarmEvent args)
		{
			if (component.ActivePlayers.Count != 0)
			{
				this.SyncAllDevices(uid);
			}
			string addr = string.Empty;
			DeviceNetworkComponent netConn;
			if (this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref netConn))
			{
				addr = netConn.Address;
			}
			if (args.AlarmType == AtmosAlarmType.Danger)
			{
				this.SetMode(uid, addr, AirAlarmMode.WideFiltering, false, null);
			}
			else if (args.AlarmType == AtmosAlarmType.Normal || args.AlarmType == AtmosAlarmType.Warning)
			{
				this.SetMode(uid, addr, AirAlarmMode.Filtering, false, null);
			}
			this.UpdateUI(uid, component, null, null);
		}

		// Token: 0x060028C1 RID: 10433 RVA: 0x000D4438 File Offset: 0x000D2638
		public void SetMode(EntityUid uid, string origin, AirAlarmMode mode, bool uiOnly = true, [Nullable(2)] AirAlarmComponent controller = null)
		{
			if (!base.Resolve<AirAlarmComponent>(uid, ref controller, true) || controller.CurrentMode == mode)
			{
				return;
			}
			controller.CurrentMode = mode;
			if (!uiOnly)
			{
				IAirAlarmMode newMode = AirAlarmModeFactory.ModeToExecutor(mode);
				if (newMode != null)
				{
					newMode.Execute(uid);
					IAirAlarmModeUpdate updatedMode = newMode as IAirAlarmModeUpdate;
					if (updatedMode != null)
					{
						controller.CurrentModeUpdater = updatedMode;
						controller.CurrentModeUpdater.NetOwner = origin;
					}
					else if (controller.CurrentModeUpdater != null)
					{
						controller.CurrentModeUpdater = null;
					}
				}
			}
			else if (controller.CurrentModeUpdater != null && controller.CurrentModeUpdater.NetOwner != origin)
			{
				controller.CurrentModeUpdater = null;
			}
			this.UpdateUI(uid, controller, null, null);
			this.SyncMode(uid, mode);
		}

		// Token: 0x060028C2 RID: 10434 RVA: 0x000D44E5 File Offset: 0x000D26E5
		private void SetDeviceData(EntityUid uid, string address, IAtmosDeviceData devData, [Nullable(2)] AirAlarmComponent controller = null)
		{
			if (!base.Resolve<AirAlarmComponent>(uid, ref controller, true))
			{
				return;
			}
			devData.Dirty = true;
			this.SetData(uid, address, devData);
		}

		// Token: 0x060028C3 RID: 10435 RVA: 0x000D4504 File Offset: 0x000D2704
		private void OnPacketRecv(EntityUid uid, AirAlarmComponent controller, DeviceNetworkPacketEvent args)
		{
			string cmd;
			if (!args.Data.TryGetValue<string>("command", out cmd))
			{
				return;
			}
			IAtmosDeviceData data;
			if (!(cmd == "atmos_sync_data"))
			{
				if (!(cmd == "air_alarm_set_mode"))
				{
					return;
				}
				AirAlarmMode alarmMode;
				if (args.Data.TryGetValue<AirAlarmMode>("air_alarm_set_mode", out alarmMode))
				{
					this.SetMode(uid, args.SenderAddress, alarmMode, false, null);
					return;
				}
			}
			else if (args.Data.TryGetValue<IAtmosDeviceData>("atmos_sync_data", out data) && controller.CanSync)
			{
				GasVentPumpData ventData = data as GasVentPumpData;
				if (ventData == null)
				{
					GasVentScrubberData scrubberData = data as GasVentScrubberData;
					if (scrubberData == null)
					{
						AtmosSensorData sensorData = data as AtmosSensorData;
						if (sensorData != null)
						{
							if (!controller.SensorData.TryAdd(args.SenderAddress, sensorData))
							{
								controller.SensorData[args.SenderAddress] = sensorData;
							}
						}
					}
					else if (!controller.ScrubberData.TryAdd(args.SenderAddress, scrubberData))
					{
						controller.ScrubberData[args.SenderAddress] = scrubberData;
					}
				}
				else if (!controller.VentData.TryAdd(args.SenderAddress, ventData))
				{
					controller.VentData[args.SenderAddress] = ventData;
				}
				controller.KnownDevices.Add(args.SenderAddress);
				this.UpdateUI(uid, controller, null, null);
				return;
			}
		}

		// Token: 0x060028C4 RID: 10436 RVA: 0x000D4646 File Offset: 0x000D2846
		private void AddActiveInterface(EntityUid uid)
		{
			this._activeUserInterfaces.Add(uid);
		}

		// Token: 0x060028C5 RID: 10437 RVA: 0x000D4655 File Offset: 0x000D2855
		private void RemoveActiveInterface(EntityUid uid)
		{
			this._activeUserInterfaces.Remove(uid);
		}

		// Token: 0x060028C6 RID: 10438 RVA: 0x000D4664 File Offset: 0x000D2864
		private void ForceCloseAllInterfaces(EntityUid uid)
		{
			this._uiSystem.TryCloseAll(uid, SharedAirAlarmInterfaceKey.Key, null);
		}

		// Token: 0x060028C7 RID: 10439 RVA: 0x000D467A File Offset: 0x000D287A
		private void OnAtmosUpdate(EntityUid uid, AirAlarmComponent alarm, AtmosDeviceUpdateEvent args)
		{
			IAirAlarmModeUpdate currentModeUpdater = alarm.CurrentModeUpdater;
			if (currentModeUpdater == null)
			{
				return;
			}
			currentModeUpdater.Update(uid);
		}

		// Token: 0x060028C8 RID: 10440 RVA: 0x000D4690 File Offset: 0x000D2890
		public float CalculatePressureAverage(AirAlarmComponent alarm)
		{
			if (alarm.SensorData.Count == 0)
			{
				return 0f;
			}
			return (from v in alarm.SensorData.Values
			select v.Pressure).Average();
		}

		// Token: 0x060028C9 RID: 10441 RVA: 0x000D46E4 File Offset: 0x000D28E4
		public float CalculateTemperatureAverage(AirAlarmComponent alarm)
		{
			if (alarm.SensorData.Count == 0)
			{
				return 0f;
			}
			return (from v in alarm.SensorData.Values
			select v.Temperature).Average();
		}

		// Token: 0x060028CA RID: 10442 RVA: 0x000D4738 File Offset: 0x000D2938
		[NullableContext(2)]
		public void UpdateUI(EntityUid uid, AirAlarmComponent alarm = null, DeviceNetworkComponent devNet = null, AtmosAlarmableComponent alarmable = null)
		{
			if (!base.Resolve<AirAlarmComponent, DeviceNetworkComponent, AtmosAlarmableComponent>(uid, ref alarm, ref devNet, ref alarmable, true))
			{
				return;
			}
			float pressure = this.CalculatePressureAverage(alarm);
			float temperature = this.CalculateTemperatureAverage(alarm);
			Dictionary<string, IAtmosDeviceData> dataToSend = new Dictionary<string, IAtmosDeviceData>();
			if (alarm.CurrentTab != AirAlarmTab.Settings)
			{
				switch (alarm.CurrentTab)
				{
				case AirAlarmTab.Vent:
					using (Dictionary<string, GasVentPumpData>.Enumerator enumerator = alarm.VentData.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, GasVentPumpData> keyValuePair = enumerator.Current;
							string text;
							GasVentPumpData gasVentPumpData;
							keyValuePair.Deconstruct(out text, out gasVentPumpData);
							string addr = text;
							GasVentPumpData data = gasVentPumpData;
							dataToSend.Add(addr, data);
						}
						goto IL_140;
					}
					break;
				case AirAlarmTab.Scrubber:
					break;
				case AirAlarmTab.Sensors:
					goto IL_F2;
				default:
					goto IL_140;
				}
				using (Dictionary<string, GasVentScrubberData>.Enumerator enumerator2 = alarm.ScrubberData.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, GasVentScrubberData> keyValuePair2 = enumerator2.Current;
						string text;
						GasVentScrubberData gasVentScrubberData;
						keyValuePair2.Deconstruct(out text, out gasVentScrubberData);
						string addr2 = text;
						GasVentScrubberData data2 = gasVentScrubberData;
						dataToSend.Add(addr2, data2);
					}
					goto IL_140;
				}
				IL_F2:
				foreach (KeyValuePair<string, AtmosSensorData> keyValuePair3 in alarm.SensorData)
				{
					string text;
					AtmosSensorData atmosSensorData;
					keyValuePair3.Deconstruct(out text, out atmosSensorData);
					string addr3 = text;
					AtmosSensorData data3 = atmosSensorData;
					dataToSend.Add(addr3, data3);
				}
			}
			IL_140:
			int deviceCount = alarm.KnownDevices.Count;
			AtmosAlarmType? highestAlarm;
			if (!this._atmosAlarmable.TryGetHighestAlert(uid, out highestAlarm, null))
			{
				highestAlarm = new AtmosAlarmType?(AtmosAlarmType.Normal);
			}
			this._uiSystem.TrySetUiState(uid, SharedAirAlarmInterfaceKey.Key, new AirAlarmUIState(devNet.Address, deviceCount, pressure, temperature, dataToSend, alarm.CurrentMode, alarm.CurrentTab, highestAlarm.Value), null, null, true);
		}

		// Token: 0x060028CB RID: 10443 RVA: 0x000D490C File Offset: 0x000D2B0C
		public override void Update(float frameTime)
		{
			this._timer += frameTime;
			if (this._timer >= 8f)
			{
				this._timer = 0f;
				foreach (EntityUid uid in this._activeUserInterfaces)
				{
					this.SyncAllSensors(uid, null);
				}
			}
		}

		// Token: 0x0400193E RID: 6462
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNet;

		// Token: 0x0400193F RID: 6463
		[Dependency]
		private readonly AtmosDeviceNetworkSystem _atmosDevNetSystem;

		// Token: 0x04001940 RID: 6464
		[Dependency]
		private readonly AtmosAlarmableSystem _atmosAlarmable;

		// Token: 0x04001941 RID: 6465
		[Dependency]
		private readonly UserInterfaceSystem _uiSystem;

		// Token: 0x04001942 RID: 6466
		[Dependency]
		private readonly AccessReaderSystem _accessSystem;

		// Token: 0x04001943 RID: 6467
		[Dependency]
		private readonly PopupSystem _popup;

		// Token: 0x04001944 RID: 6468
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04001945 RID: 6469
		public const string AirAlarmSetMode = "air_alarm_set_mode";

		// Token: 0x04001946 RID: 6470
		private readonly HashSet<EntityUid> _activeUserInterfaces = new HashSet<EntityUid>();

		// Token: 0x04001947 RID: 6471
		private const float Delay = 8f;

		// Token: 0x04001948 RID: 6472
		private float _timer;
	}
}
