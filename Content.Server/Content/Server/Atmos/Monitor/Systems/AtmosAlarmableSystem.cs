using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Power.Components;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x0200077F RID: 1919
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosAlarmableSystem : EntitySystem
	{
		// Token: 0x060028CD RID: 10445 RVA: 0x000D499B File Offset: 0x000D2B9B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AtmosAlarmableComponent, MapInitEvent>(new ComponentEventHandler<AtmosAlarmableComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<AtmosAlarmableComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<AtmosAlarmableComponent, DeviceNetworkPacketEvent>(this.OnPacketRecv), null, null);
			base.SubscribeLocalEvent<AtmosAlarmableComponent, PowerChangedEvent>(new ComponentEventRefHandler<AtmosAlarmableComponent, PowerChangedEvent>(this.OnPowerChange), null, null);
		}

		// Token: 0x060028CE RID: 10446 RVA: 0x000D49DC File Offset: 0x000D2BDC
		private void OnMapInit(EntityUid uid, AtmosAlarmableComponent component, MapInitEvent args)
		{
			AtmosAlarmType? alarm;
			this.TryUpdateAlert(uid, this.TryGetHighestAlert(uid, out alarm, null) ? alarm.Value : AtmosAlarmType.Normal, component, false);
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x000D4A08 File Offset: 0x000D2C08
		private void OnPowerChange(EntityUid uid, AtmosAlarmableComponent component, ref PowerChangedEvent args)
		{
			if (!args.Powered)
			{
				this.Reset(uid, component, null);
				return;
			}
			this._atmosDevNetSystem.Register(uid, null);
			this._atmosDevNetSystem.Sync(uid, null);
			AtmosAlarmType? alarm;
			this.TryUpdateAlert(uid, this.TryGetHighestAlert(uid, out alarm, null) ? alarm.Value : AtmosAlarmType.Normal, component, false);
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x000D4A60 File Offset: 0x000D2C60
		private void OnPacketRecv(EntityUid uid, AtmosAlarmableComponent component, DeviceNetworkPacketEvent args)
		{
			if (component.IgnoreAlarms)
			{
				return;
			}
			DeviceNetworkComponent netConn;
			if (!this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref netConn))
			{
				return;
			}
			string cmd;
			HashSet<string> sourceTags;
			if (!args.Data.TryGetValue<string>("command", out cmd) || !args.Data.TryGetValue<HashSet<string>>("atmos_alarm_source", out sourceTags))
			{
				return;
			}
			bool isValid = sourceTags.Any((string source) => component.SyncWithTags.Contains(source));
			if (!isValid)
			{
				return;
			}
			AtmosAlarmType state;
			if (!(cmd == "atmos_alarm"))
			{
				if (cmd == "atmos_alarmable_reset_all")
				{
					this.Reset(uid, component, null);
					return;
				}
				if (!(cmd == "atmos_alarmable_sync_alerts"))
				{
					return;
				}
				IReadOnlyDictionary<string, AtmosAlarmType> alarms;
				if (args.Data.TryGetValue<IReadOnlyDictionary<string, AtmosAlarmType>>("atmos_alarmable_sync_alerts", out alarms))
				{
					foreach (KeyValuePair<string, AtmosAlarmType> keyValuePair in alarms)
					{
						string text;
						AtmosAlarmType atmosAlarmType;
						keyValuePair.Deconstruct(out text, out atmosAlarmType);
						string key = text;
						AtmosAlarmType alarm = atmosAlarmType;
						if (!component.NetworkAlarmStates.TryAdd(key, alarm))
						{
							component.NetworkAlarmStates[key] = alarm;
						}
					}
					AtmosAlarmType? maxAlert;
					if (this.TryGetHighestAlert(uid, out maxAlert, component))
					{
						this.TryUpdateAlert(uid, maxAlert.Value, component, true);
					}
				}
			}
			else if (args.Data.TryGetValue<AtmosAlarmType>("set_state", out state))
			{
				HashSet<AtmosMonitorThresholdType> types;
				if (args.Data.TryGetValue<HashSet<AtmosMonitorThresholdType>>("atmos_alarm_types", out types) && component.MonitorAlertTypes != null)
				{
					isValid = types.Any((AtmosMonitorThresholdType type) => component.MonitorAlertTypes.Contains(type));
				}
				if (!component.NetworkAlarmStates.ContainsKey(args.SenderAddress))
				{
					if (!isValid)
					{
						return;
					}
					component.NetworkAlarmStates.Add(args.SenderAddress, state);
				}
				else
				{
					component.NetworkAlarmStates[args.SenderAddress] = (isValid ? state : AtmosAlarmType.Normal);
				}
				AtmosAlarmType? netMax;
				if (!this.TryGetHighestAlert(uid, out netMax, component))
				{
					netMax = new AtmosAlarmType?(AtmosAlarmType.Normal);
				}
				this.TryUpdateAlert(uid, netMax.Value, component, true);
				return;
			}
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000D4CA4 File Offset: 0x000D2EA4
		private void TryUpdateAlert(EntityUid uid, AtmosAlarmType type, AtmosAlarmableComponent alarmable, bool sync = true)
		{
			if (alarmable.LastAlarmState == type)
			{
				return;
			}
			if (sync)
			{
				this.SyncAlertsToNetwork(uid, null, alarmable, null);
			}
			alarmable.LastAlarmState = type;
			this.UpdateAppearance(uid, type);
			this.PlayAlertSound(uid, type, alarmable);
			base.RaiseLocalEvent<AtmosAlarmEvent>(uid, new AtmosAlarmEvent(type), true);
		}

		// Token: 0x060028D2 RID: 10450 RVA: 0x000D4CE4 File Offset: 0x000D2EE4
		[NullableContext(2)]
		public void SyncAlertsToNetwork(EntityUid uid, string address = null, AtmosAlarmableComponent alarmable = null, TagComponent tags = null)
		{
			if (!base.Resolve<AtmosAlarmableComponent, TagComponent>(uid, ref alarmable, ref tags, true) || alarmable.ReceiveOnly)
			{
				return;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_alarmable_sync_alerts";
			networkPayload["atmos_alarmable_sync_alerts"] = alarmable.NetworkAlarmStates;
			networkPayload["atmos_alarm_source"] = tags.Tags;
			NetworkPayload payload = networkPayload;
			this._deviceNet.QueuePacket(uid, address, payload, null, null);
		}

		// Token: 0x060028D3 RID: 10451 RVA: 0x000D4D5C File Offset: 0x000D2F5C
		[NullableContext(2)]
		public void ForceAlert(EntityUid uid, AtmosAlarmType alarmType, AtmosAlarmableComponent alarmable = null, DeviceNetworkComponent devNet = null, TagComponent tags = null)
		{
			if (!base.Resolve<AtmosAlarmableComponent, DeviceNetworkComponent, TagComponent>(uid, ref alarmable, ref devNet, ref tags, true))
			{
				return;
			}
			this.TryUpdateAlert(uid, alarmType, alarmable, false);
			if (alarmable.ReceiveOnly)
			{
				return;
			}
			if (!alarmable.NetworkAlarmStates.TryAdd(devNet.Address, alarmType))
			{
				alarmable.NetworkAlarmStates[devNet.Address] = alarmType;
			}
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_alarm";
			networkPayload["set_state"] = alarmType;
			networkPayload["atmos_alarm_source"] = tags.Tags;
			NetworkPayload payload = networkPayload;
			this._deviceNet.QueuePacket(uid, null, payload, null, null);
		}

		// Token: 0x060028D4 RID: 10452 RVA: 0x000D4E08 File Offset: 0x000D3008
		[NullableContext(2)]
		public void Reset(EntityUid uid, AtmosAlarmableComponent alarmable = null, TagComponent tags = null)
		{
			if (!base.Resolve<AtmosAlarmableComponent, TagComponent>(uid, ref alarmable, ref tags, false) || alarmable.LastAlarmState == AtmosAlarmType.Normal)
			{
				return;
			}
			alarmable.NetworkAlarmStates.Clear();
			this.TryUpdateAlert(uid, AtmosAlarmType.Normal, alarmable, true);
			if (!alarmable.ReceiveOnly)
			{
				NetworkPayload networkPayload = new NetworkPayload();
				networkPayload["command"] = "atmos_alarmable_reset_all";
				networkPayload["atmos_alarm_source"] = tags.Tags;
				NetworkPayload payload = networkPayload;
				this._deviceNet.QueuePacket(uid, null, payload, null, null);
			}
		}

		// Token: 0x060028D5 RID: 10453 RVA: 0x000D4E8A File Offset: 0x000D308A
		[NullableContext(2)]
		public void ResetAllOnNetwork(EntityUid uid, AtmosAlarmableComponent alarmable = null)
		{
			if (!base.Resolve<AtmosAlarmableComponent>(uid, ref alarmable, true) || alarmable.ReceiveOnly)
			{
				return;
			}
			this.Reset(uid, alarmable, null);
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x000D4EAC File Offset: 0x000D30AC
		[NullableContext(2)]
		public bool TryGetHighestAlert(EntityUid uid, [NotNullWhen(true)] out AtmosAlarmType? alarm, AtmosAlarmableComponent alarmable = null)
		{
			alarm = null;
			if (!base.Resolve<AtmosAlarmableComponent>(uid, ref alarmable, false))
			{
				return false;
			}
			foreach (AtmosAlarmType alarmState in alarmable.NetworkAlarmStates.Values)
			{
				if (alarm == null)
				{
					goto IL_5E;
				}
				AtmosAlarmType? atmosAlarmType = alarm;
				AtmosAlarmType atmosAlarmType2 = alarmState;
				if (atmosAlarmType.GetValueOrDefault() < atmosAlarmType2 & atmosAlarmType != null)
				{
					goto IL_5E;
				}
				AtmosAlarmType? atmosAlarmType3 = alarm;
				IL_64:
				alarm = atmosAlarmType3;
				continue;
				IL_5E:
				atmosAlarmType3 = new AtmosAlarmType?(alarmState);
				goto IL_64;
			}
			return alarm != null;
		}

		// Token: 0x060028D7 RID: 10455 RVA: 0x000D4F54 File Offset: 0x000D3154
		private void PlayAlertSound(EntityUid uid, AtmosAlarmType alarm, AtmosAlarmableComponent alarmable)
		{
			if (alarm == AtmosAlarmType.Danger)
			{
				this._audioSystem.PlayPvs(alarmable.AlarmSound, uid, new AudioParams?(AudioParams.Default.WithVolume(alarmable.AlarmVolume)));
			}
		}

		// Token: 0x060028D8 RID: 10456 RVA: 0x000D4F82 File Offset: 0x000D3182
		private void UpdateAppearance(EntityUid uid, AtmosAlarmType alarm)
		{
			this._appearance.SetData(uid, AtmosMonitorVisuals.AlarmType, alarm, null);
		}

		// Token: 0x04001949 RID: 6473
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x0400194A RID: 6474
		[Dependency]
		private readonly AudioSystem _audioSystem;

		// Token: 0x0400194B RID: 6475
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNet;

		// Token: 0x0400194C RID: 6476
		[Dependency]
		private readonly AtmosDeviceNetworkSystem _atmosDevNetSystem;

		// Token: 0x0400194D RID: 6477
		public const string AlertCmd = "atmos_alarm";

		// Token: 0x0400194E RID: 6478
		public const string AlertSource = "atmos_alarm_source";

		// Token: 0x0400194F RID: 6479
		public const string AlertTypes = "atmos_alarm_types";

		// Token: 0x04001950 RID: 6480
		public const string SyncAlerts = "atmos_alarmable_sync_alerts";

		// Token: 0x04001951 RID: 6481
		public const string ResetAll = "atmos_alarmable_reset_all";
	}
}
