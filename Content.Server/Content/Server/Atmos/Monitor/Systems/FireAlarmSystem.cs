using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Monitor.Components;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Access.Systems;
using Content.Shared.Atmos.Monitor;
using Content.Shared.CCVar;
using Content.Shared.DeviceNetwork;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x02000783 RID: 1923
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FireAlarmSystem : EntitySystem
	{
		// Token: 0x060028EF RID: 10479 RVA: 0x000D589E File Offset: 0x000D3A9E
		public override void Initialize()
		{
			base.SubscribeLocalEvent<FireAlarmComponent, InteractHandEvent>(new ComponentEventHandler<FireAlarmComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<FireAlarmComponent, DeviceListUpdateEvent>(new ComponentEventHandler<FireAlarmComponent, DeviceListUpdateEvent>(this.OnDeviceListSync), null, null);
			base.SubscribeLocalEvent<FireAlarmComponent, GotEmaggedEvent>(new ComponentEventRefHandler<FireAlarmComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
		}

		// Token: 0x060028F0 RID: 10480 RVA: 0x000D58DC File Offset: 0x000D3ADC
		private void OnDeviceListSync(EntityUid uid, FireAlarmComponent component, DeviceListUpdateEvent args)
		{
			EntityQuery<DeviceNetworkComponent> query = base.GetEntityQuery<DeviceNetworkComponent>();
			foreach (EntityUid device in args.OldDevices)
			{
				DeviceNetworkComponent deviceNet;
				if (query.TryGetComponent(device, ref deviceNet))
				{
					this._atmosDevNet.Deregister(uid, deviceNet.Address);
				}
			}
			this._atmosDevNet.Register(uid, null);
			this._atmosDevNet.Sync(uid, null);
		}

		// Token: 0x060028F1 RID: 10481 RVA: 0x000D5968 File Offset: 0x000D3B68
		private void OnInteractHand(EntityUid uid, FireAlarmComponent component, InteractHandEvent args)
		{
			if (!this._interactionSystem.InRangeUnobstructed(args.User, args.Target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return;
			}
			if (!this._configManager.GetCVar<bool>(CCVars.FireAlarmAllAccess) && !this._access.IsAllowed(args.User, args.Target, null))
			{
				return;
			}
			if (this.IsPowered(uid, this.EntityManager, null))
			{
				AtmosAlarmType? alarm;
				if (!this._atmosAlarmable.TryGetHighestAlert(uid, out alarm, null))
				{
					alarm = new AtmosAlarmType?(AtmosAlarmType.Normal);
				}
				AtmosAlarmType? atmosAlarmType = alarm;
				AtmosAlarmType atmosAlarmType2 = AtmosAlarmType.Normal;
				if (atmosAlarmType.GetValueOrDefault() == atmosAlarmType2 & atmosAlarmType != null)
				{
					this._atmosAlarmable.ForceAlert(uid, AtmosAlarmType.Danger, null, null, null);
					return;
				}
				this._atmosAlarmable.ResetAllOnNetwork(uid, null);
			}
		}

		// Token: 0x060028F2 RID: 10482 RVA: 0x000D5A28 File Offset: 0x000D3C28
		private void OnEmagged(EntityUid uid, FireAlarmComponent component, ref GotEmaggedEvent args)
		{
			AtmosAlarmableComponent alarmable;
			if (base.TryComp<AtmosAlarmableComponent>(uid, ref alarmable))
			{
				this._atmosAlarmable.ForceAlert(uid, AtmosAlarmType.Emagged, alarmable, null, null);
				base.RemCompDeferred<AtmosAlarmableComponent>(uid);
			}
		}

		// Token: 0x0400195F RID: 6495
		[Dependency]
		private readonly AtmosDeviceNetworkSystem _atmosDevNet;

		// Token: 0x04001960 RID: 6496
		[Dependency]
		private readonly AtmosAlarmableSystem _atmosAlarmable;

		// Token: 0x04001961 RID: 6497
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04001962 RID: 6498
		[Dependency]
		private readonly AccessReaderSystem _access;

		// Token: 0x04001963 RID: 6499
		[Dependency]
		private readonly IConfigurationManager _configManager;
	}
}
