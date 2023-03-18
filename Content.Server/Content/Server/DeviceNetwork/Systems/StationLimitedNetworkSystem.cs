using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Station.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000588 RID: 1416
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StationLimitedNetworkSystem : EntitySystem
	{
		// Token: 0x06001DB7 RID: 7607 RVA: 0x0009E65C File Offset: 0x0009C85C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StationLimitedNetworkComponent, MapInitEvent>(new ComponentEventHandler<StationLimitedNetworkComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<StationLimitedNetworkComponent, BeforePacketSentEvent>(new ComponentEventHandler<StationLimitedNetworkComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0009E68C File Offset: 0x0009C88C
		[NullableContext(2)]
		public void SetStation(EntityUid uid, EntityUid? stationId, StationLimitedNetworkComponent component = null)
		{
			if (!base.Resolve<StationLimitedNetworkComponent>(uid, ref component, true))
			{
				return;
			}
			component.StationId = stationId;
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x0009E6A2 File Offset: 0x0009C8A2
		private void OnMapInit(EntityUid uid, StationLimitedNetworkComponent networkComponent, MapInitEvent args)
		{
			networkComponent.StationId = this._stationSystem.GetOwningStation(uid, null);
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x0009E6B7 File Offset: 0x0009C8B7
		private void OnBeforePacketSent(EntityUid uid, StationLimitedNetworkComponent component, BeforePacketSentEvent args)
		{
			if (!this.CheckStationId(args.Sender, component.AllowNonStationPackets, component.StationId, null))
			{
				args.Cancel();
			}
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x0009E6DC File Offset: 0x0009C8DC
		[NullableContext(2)]
		private bool CheckStationId(EntityUid senderUid, bool allowNonStationPackets, EntityUid? receiverStationId, StationLimitedNetworkComponent sender = null)
		{
			if (receiverStationId == null)
			{
				return false;
			}
			if (!base.Resolve<StationLimitedNetworkComponent>(senderUid, ref sender, false))
			{
				return allowNonStationPackets;
			}
			return sender.StationId == receiverStationId;
		}

		// Token: 0x04001309 RID: 4873
		[Dependency]
		private readonly StationSystem _stationSystem;
	}
}
