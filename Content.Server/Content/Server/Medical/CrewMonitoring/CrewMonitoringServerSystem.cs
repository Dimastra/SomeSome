using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Medical.SuitSensors;
using Content.Server.Power.Components;
using Content.Server.Station.Systems;
using Content.Shared.Medical.SuitSensor;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Medical.CrewMonitoring
{
	// Token: 0x020003BA RID: 954
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrewMonitoringServerSystem : EntitySystem
	{
		// Token: 0x060013AA RID: 5034 RVA: 0x00065C80 File Offset: 0x00063E80
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<CrewMonitoringServerComponent, ComponentRemove>(new ComponentEventHandler<CrewMonitoringServerComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<CrewMonitoringServerComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<CrewMonitoringServerComponent, DeviceNetworkPacketEvent>(this.OnPacketReceived), null, null);
			base.SubscribeLocalEvent<CrewMonitoringServerComponent, PowerChangedEvent>(new ComponentEventRefHandler<CrewMonitoringServerComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x00065CD0 File Offset: 0x00063ED0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._updateDiff += frameTime;
			if (this._updateDiff < 3f)
			{
				return;
			}
			this._updateDiff -= 3f;
			IEnumerable<CrewMonitoringServerComponent> enumerable = this.EntityManager.EntityQuery<CrewMonitoringServerComponent>(false);
			List<EntityUid> activeServers = new List<EntityUid>();
			foreach (CrewMonitoringServerComponent server in enumerable)
			{
				if (!server.Available)
				{
					if (server.Active)
					{
						this.DisconnectServer(server.Owner, server, null);
					}
				}
				else if (server.Active)
				{
					activeServers.Add(server.Owner);
				}
			}
			foreach (EntityUid activeServer in activeServers)
			{
				this.UpdateTimeout(activeServer, null);
				this.BroadcastSensorStatus(activeServer, null, null);
			}
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x00065DD8 File Offset: 0x00063FD8
		[NullableContext(2)]
		public bool TryGetActiveServerAddress(EntityUid stationId, out string address)
		{
			IEnumerable<ValueTuple<CrewMonitoringServerComponent, DeviceNetworkComponent>> enumerable = this.EntityManager.EntityQuery<CrewMonitoringServerComponent, DeviceNetworkComponent>(false);
			ValueTuple<CrewMonitoringServerComponent, DeviceNetworkComponent>? last = null;
			foreach (ValueTuple<CrewMonitoringServerComponent, DeviceNetworkComponent> valueTuple in enumerable)
			{
				CrewMonitoringServerComponent server = valueTuple.Item1;
				DeviceNetworkComponent device = valueTuple.Item2;
				EntityUid? entityUid;
				if (this._stationSystem.GetOwningStation(server.Owner, null) == null || entityUid.GetValueOrDefault().Equals(stationId))
				{
					if (!server.Available)
					{
						this.DisconnectServer(server.Owner, server, device);
					}
					else
					{
						last = new ValueTuple<CrewMonitoringServerComponent, DeviceNetworkComponent>?(new ValueTuple<CrewMonitoringServerComponent, DeviceNetworkComponent>(server, device));
						if (server.Active)
						{
							address = device.Address;
							return true;
						}
					}
				}
			}
			if (last != null)
			{
				this.ConnectServer(last.Value.Item1.Owner, last.Value.Item1, last.Value.Item2);
				address = last.Value.Item2.Address;
				return true;
			}
			address = null;
			return address != null;
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x00065F08 File Offset: 0x00064108
		private void OnPacketReceived(EntityUid uid, CrewMonitoringServerComponent component, DeviceNetworkPacketEvent args)
		{
			SuitSensorStatus sensorStatus = this._sensors.PacketToSuitSensor(args.Data);
			if (sensorStatus == null)
			{
				return;
			}
			sensorStatus.Timestamp = this._gameTiming.CurTime;
			component.SensorStatus[args.SenderAddress] = sensorStatus;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x00065F4E File Offset: 0x0006414E
		private void OnRemove(EntityUid uid, CrewMonitoringServerComponent component, ComponentRemove args)
		{
			component.SensorStatus.Clear();
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00065F5B File Offset: 0x0006415B
		private void OnPowerChanged(EntityUid uid, CrewMonitoringServerComponent component, ref PowerChangedEvent args)
		{
			component.Available = args.Powered;
			if (!args.Powered)
			{
				this.DisconnectServer(uid, component, null);
			}
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00065F7C File Offset: 0x0006417C
		[NullableContext(2)]
		private void UpdateTimeout(EntityUid uid, CrewMonitoringServerComponent component = null)
		{
			if (!base.Resolve<CrewMonitoringServerComponent>(uid, ref component, true))
			{
				return;
			}
			foreach (KeyValuePair<string, SuitSensorStatus> keyValuePair in component.SensorStatus)
			{
				string text;
				SuitSensorStatus suitSensorStatus;
				keyValuePair.Deconstruct(out text, out suitSensorStatus);
				string address = text;
				SuitSensorStatus sensor = suitSensorStatus;
				if ((float)(this._gameTiming.CurTime - sensor.Timestamp).Seconds > component.SensorTimeout)
				{
					component.SensorStatus.Remove(address);
				}
			}
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x0006601C File Offset: 0x0006421C
		[NullableContext(2)]
		private void BroadcastSensorStatus(EntityUid uid, CrewMonitoringServerComponent serverComponent = null, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<CrewMonitoringServerComponent, DeviceNetworkComponent>(uid, ref serverComponent, ref device, true))
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "updated_state";
			networkPayload["suit-status-collection"] = serverComponent.SensorStatus;
			NetworkPayload payload = networkPayload;
			DeviceNetworkSystem deviceNetworkSystem = this._deviceNetworkSystem;
			string address = null;
			NetworkPayload data = payload;
			DeviceNetworkComponent device2 = device;
			deviceNetworkSystem.QueuePacket(uid, address, data, null, device2);
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x0006607A File Offset: 0x0006427A
		[NullableContext(2)]
		private void ConnectServer(EntityUid uid, CrewMonitoringServerComponent server = null, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<CrewMonitoringServerComponent, DeviceNetworkComponent>(uid, ref server, ref device, true))
			{
				return;
			}
			server.Active = true;
			if (this._deviceNetworkSystem.IsDeviceConnected(uid, device))
			{
				return;
			}
			this._deviceNetworkSystem.ConnectDevice(uid, device);
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x000660B0 File Offset: 0x000642B0
		[NullableContext(2)]
		private void DisconnectServer(EntityUid uid, CrewMonitoringServerComponent server = null, DeviceNetworkComponent device = null)
		{
			if (!base.Resolve<CrewMonitoringServerComponent, DeviceNetworkComponent>(uid, ref server, ref device, true))
			{
				return;
			}
			server.SensorStatus.Clear();
			server.Active = false;
			this._deviceNetworkSystem.DisconnectDevice(uid, device, false);
		}

		// Token: 0x04000C07 RID: 3079
		[Dependency]
		private readonly SuitSensorSystem _sensors;

		// Token: 0x04000C08 RID: 3080
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000C09 RID: 3081
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x04000C0A RID: 3082
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000C0B RID: 3083
		private const float UpdateRate = 3f;

		// Token: 0x04000C0C RID: 3084
		private float _updateDiff;
	}
}
