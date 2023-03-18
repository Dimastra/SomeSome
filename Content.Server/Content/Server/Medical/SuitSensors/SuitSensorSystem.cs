using System;
using System.Runtime.CompilerServices;
using Content.Server.Access.Systems;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Medical.CrewMonitoring;
using Content.Server.Popups;
using Content.Server.Station.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory.Events;
using Content.Shared.Medical.SuitSensor;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Medical.SuitSensors
{
	// Token: 0x020003B6 RID: 950
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SuitSensorSystem : EntitySystem
	{
		// Token: 0x06001392 RID: 5010 RVA: 0x000652B0 File Offset: 0x000634B0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SuitSensorComponent, MapInitEvent>(new ComponentEventHandler<SuitSensorComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, GotEquippedEvent>(new ComponentEventHandler<SuitSensorComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, GotUnequippedEvent>(new ComponentEventHandler<SuitSensorComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, ExaminedEvent>(new ComponentEventHandler<SuitSensorComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<SuitSensorComponent, GetVerbsEvent<Verb>>(this.OnVerb), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<SuitSensorComponent, EntGotInsertedIntoContainerMessage>(this.OnInsert), null, null);
			base.SubscribeLocalEvent<SuitSensorComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<SuitSensorComponent, EntGotRemovedFromContainerMessage>(this.OnRemove), null, null);
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x00065350 File Offset: 0x00063550
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._updateDif += frameTime;
			if (this._updateDif < 1f)
			{
				return;
			}
			this._updateDif -= 1f;
			TimeSpan curTime = this._gameTiming.CurTime;
			foreach (ValueTuple<SuitSensorComponent, DeviceNetworkComponent> valueTuple in this.EntityManager.EntityQuery<SuitSensorComponent, DeviceNetworkComponent>(false))
			{
				SuitSensorComponent sensor = valueTuple.Item1;
				DeviceNetworkComponent device = valueTuple.Item2;
				if (device.TransmitFrequency != null && sensor.StationId != null && !(curTime - sensor.LastUpdate < sensor.UpdateRate))
				{
					sensor.LastUpdate = curTime.Add(TimeSpan.FromSeconds((double)this._random.Next(0, sensor.UpdateRate.Seconds)));
					SuitSensorStatus status = this.GetSensorState(sensor.Owner, sensor, null);
					if (status != null)
					{
						if (sensor.ConnectedServer == null)
						{
							string address;
							if (!this._monitoringServerSystem.TryGetActiveServerAddress(sensor.StationId.Value, out address))
							{
								continue;
							}
							sensor.ConnectedServer = address;
						}
						NetworkPayload payload = this.SuitSensorToPacket(status);
						if (!this._deviceNetworkSystem.IsAddressPresent(device.DeviceNetId, sensor.ConnectedServer))
						{
							sensor.ConnectedServer = null;
						}
						else
						{
							DeviceNetworkSystem deviceNetworkSystem = this._deviceNetworkSystem;
							EntityUid owner = sensor.Owner;
							string connectedServer = sensor.ConnectedServer;
							NetworkPayload data = payload;
							DeviceNetworkComponent device2 = device;
							deviceNetworkSystem.QueuePacket(owner, connectedServer, data, null, device2);
						}
					}
				}
			}
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x000654F4 File Offset: 0x000636F4
		private void OnMapInit(EntityUid uid, SuitSensorComponent component, MapInitEvent args)
		{
			component.StationId = this._stationSystem.GetOwningStation(uid, null);
			if (component.RandomMode)
			{
				SuitSensorMode[] array = new SuitSensorMode[8];
				RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.CE78FEE9A167BF3809FF74F26E79092AC76D61CA6FDB0DC8E06E4E3F724D6E81).FieldHandle);
				SuitSensorMode[] modesDist = array;
				component.Mode = RandomExtensions.Pick<SuitSensorMode>(this._random, modesDist);
			}
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x00065540 File Offset: 0x00063740
		private void OnEquipped(EntityUid uid, SuitSensorComponent component, GotEquippedEvent args)
		{
			if (args.Slot != component.ActivationSlot)
			{
				return;
			}
			component.User = new EntityUid?(args.Equipee);
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x00065567 File Offset: 0x00063767
		private void OnUnequipped(EntityUid uid, SuitSensorComponent component, GotUnequippedEvent args)
		{
			if (args.Slot != component.ActivationSlot)
			{
				return;
			}
			component.User = null;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0006558C File Offset: 0x0006378C
		private void OnExamine(EntityUid uid, SuitSensorComponent component, ExaminedEvent args)
		{
			if (!args.IsInDetailsRange)
			{
				return;
			}
			string msg;
			switch (component.Mode)
			{
			case SuitSensorMode.SensorOff:
				msg = "suit-sensor-examine-off";
				break;
			case SuitSensorMode.SensorBinary:
				msg = "suit-sensor-examine-binary";
				break;
			case SuitSensorMode.SensorVitals:
				msg = "suit-sensor-examine-vitals";
				break;
			case SuitSensorMode.SensorCords:
				msg = "suit-sensor-examine-cords";
				break;
			default:
				return;
			}
			args.PushMarkup(Loc.GetString(msg));
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x000655EC File Offset: 0x000637EC
		private void OnVerb(EntityUid uid, SuitSensorComponent component, GetVerbsEvent<Verb> args)
		{
			if (component.ControlsLocked)
			{
				return;
			}
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			args.Verbs.UnionWith(new Verb[]
			{
				this.CreateVerb(uid, component, args.User, SuitSensorMode.SensorOff),
				this.CreateVerb(uid, component, args.User, SuitSensorMode.SensorBinary),
				this.CreateVerb(uid, component, args.User, SuitSensorMode.SensorVitals),
				this.CreateVerb(uid, component, args.User, SuitSensorMode.SensorCords)
			});
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00065674 File Offset: 0x00063874
		private void OnInsert(EntityUid uid, SuitSensorComponent component, EntGotInsertedIntoContainerMessage args)
		{
			if (args.Container.ID != component.ActivationContainer)
			{
				return;
			}
			component.User = new EntityUid?(args.Container.Owner);
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x000656A5 File Offset: 0x000638A5
		private void OnRemove(EntityUid uid, SuitSensorComponent component, EntGotRemovedFromContainerMessage args)
		{
			if (args.Container.ID != component.ActivationContainer)
			{
				return;
			}
			component.User = null;
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x000656CC File Offset: 0x000638CC
		private Verb CreateVerb(EntityUid uid, SuitSensorComponent component, EntityUid userUid, SuitSensorMode mode)
		{
			return new Verb
			{
				Text = this.GetModeName(mode),
				Disabled = (component.Mode == mode),
				Priority = (int)(-(int)mode),
				Category = VerbCategory.SetSensor,
				Act = delegate()
				{
					this.SetSensor(uid, mode, new EntityUid?(userUid), component);
				}
			};
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x00065760 File Offset: 0x00063960
		private string GetModeName(SuitSensorMode mode)
		{
			string name;
			switch (mode)
			{
			case SuitSensorMode.SensorOff:
				name = "suit-sensor-mode-off";
				break;
			case SuitSensorMode.SensorBinary:
				name = "suit-sensor-mode-binary";
				break;
			case SuitSensorMode.SensorVitals:
				name = "suit-sensor-mode-vitals";
				break;
			case SuitSensorMode.SensorCords:
				name = "suit-sensor-mode-cords";
				break;
			default:
				return "";
			}
			return Loc.GetString(name);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x000657B4 File Offset: 0x000639B4
		[NullableContext(2)]
		public void SetSensor(EntityUid uid, SuitSensorMode mode, EntityUid? userUid = null, SuitSensorComponent component = null)
		{
			if (!base.Resolve<SuitSensorComponent>(uid, ref component, true))
			{
				return;
			}
			component.Mode = mode;
			if (userUid != null)
			{
				string msg = Loc.GetString("suit-sensor-mode-state", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("mode", this.GetModeName(mode))
				});
				this._popupSystem.PopupEntity(msg, uid, userUid.Value, PopupType.Small);
			}
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00065820 File Offset: 0x00063A20
		[NullableContext(2)]
		public SuitSensorStatus GetSensorState(EntityUid uid, SuitSensorComponent sensor = null, TransformComponent transform = null)
		{
			if (!base.Resolve<SuitSensorComponent, TransformComponent>(uid, ref sensor, ref transform, true))
			{
				return null;
			}
			if (sensor.Mode == SuitSensorMode.SensorOff || sensor.User == null || transform.GridUid == null)
			{
				return null;
			}
			string userName = Loc.GetString("suit-sensor-component-unknown-name");
			string userJob = Loc.GetString("suit-sensor-component-unknown-job");
			IdCardComponent card;
			if (this._idCardSystem.TryFindIdCard(sensor.User.Value, out card))
			{
				if (card.FullName != null)
				{
					userName = card.FullName;
				}
				if (card.JobTitle != null)
				{
					userJob = card.JobTitle;
				}
			}
			bool isAlive = false;
			MobStateComponent mobState;
			if (this.EntityManager.TryGetComponent<MobStateComponent>(sensor.User.Value, ref mobState))
			{
				isAlive = this._mobStateSystem.IsAlive(sensor.User.Value, mobState);
			}
			int totalDamage = 0;
			DamageableComponent damageable;
			if (base.TryComp<DamageableComponent>(sensor.User.Value, ref damageable))
			{
				totalDamage = damageable.TotalDamage.Int();
			}
			TransformComponent xForm = base.Transform(sensor.User.Value);
			EntityQuery<TransformComponent> xFormQuery = base.GetEntityQuery<TransformComponent>();
			EntityCoordinates coords = this._xform.GetMoverCoordinates(xForm, xFormQuery);
			SuitSensorStatus status = new SuitSensorStatus(userName, userJob);
			switch (sensor.Mode)
			{
			case SuitSensorMode.SensorBinary:
				status.IsAlive = isAlive;
				break;
			case SuitSensorMode.SensorVitals:
				status.IsAlive = isAlive;
				status.TotalDamage = new int?(totalDamage);
				break;
			case SuitSensorMode.SensorCords:
				status.IsAlive = isAlive;
				status.TotalDamage = new int?(totalDamage);
				status.Coordinates = new EntityCoordinates?(coords);
				break;
			}
			return status;
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x000659AC File Offset: 0x00063BAC
		public NetworkPayload SuitSensorToPacket(SuitSensorStatus status)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "updated_state";
			networkPayload["name"] = status.Name;
			networkPayload["job"] = status.Job;
			networkPayload["alive"] = status.IsAlive;
			NetworkPayload payload = networkPayload;
			if (status.TotalDamage != null)
			{
				payload.Add("vitals", status.TotalDamage);
			}
			if (status.Coordinates != null)
			{
				payload.Add("cords", status.Coordinates);
			}
			return payload;
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x00065A50 File Offset: 0x00063C50
		[return: Nullable(2)]
		public SuitSensorStatus PacketToSuitSensor(NetworkPayload payload)
		{
			string command;
			if (!payload.TryGetValue<string>("command", out command))
			{
				return null;
			}
			if (command != "updated_state")
			{
				return null;
			}
			string name;
			if (!payload.TryGetValue<string>("name", out name))
			{
				return null;
			}
			string job;
			if (!payload.TryGetValue<string>("job", out job))
			{
				return null;
			}
			bool? isAlive;
			if (!payload.TryGetValue<bool?>("alive", out isAlive))
			{
				return null;
			}
			int? totalDamage;
			payload.TryGetValue<int?>("vitals", out totalDamage);
			EntityCoordinates? cords;
			payload.TryGetValue<EntityCoordinates?>("cords", out cords);
			return new SuitSensorStatus(name, job)
			{
				IsAlive = isAlive.Value,
				TotalDamage = totalDamage,
				Coordinates = cords
			};
		}

		// Token: 0x04000BF0 RID: 3056
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000BF1 RID: 3057
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000BF2 RID: 3058
		[Dependency]
		private readonly IdCardSystem _idCardSystem;

		// Token: 0x04000BF3 RID: 3059
		[Dependency]
		private readonly MobStateSystem _mobStateSystem;

		// Token: 0x04000BF4 RID: 3060
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetworkSystem;

		// Token: 0x04000BF5 RID: 3061
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000BF6 RID: 3062
		[Dependency]
		private readonly CrewMonitoringServerSystem _monitoringServerSystem;

		// Token: 0x04000BF7 RID: 3063
		[Dependency]
		private readonly StationSystem _stationSystem;

		// Token: 0x04000BF8 RID: 3064
		[Dependency]
		private readonly SharedTransformSystem _xform;

		// Token: 0x04000BF9 RID: 3065
		private const float UpdateRate = 1f;

		// Token: 0x04000BFA RID: 3066
		private float _updateDif;
	}
}
