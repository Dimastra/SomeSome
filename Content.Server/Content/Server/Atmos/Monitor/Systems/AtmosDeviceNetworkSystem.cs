using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x02000781 RID: 1921
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosDeviceNetworkSystem : EntitySystem
	{
		// Token: 0x060028DC RID: 10460 RVA: 0x000D4FBC File Offset: 0x000D31BC
		[NullableContext(2)]
		public void Register(EntityUid uid, string address)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_register_device";
			NetworkPayload registerPayload = networkPayload;
			this._deviceNet.QueuePacket(uid, address, registerPayload, null, null);
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x000D4FF8 File Offset: 0x000D31F8
		[NullableContext(2)]
		public void Deregister(EntityUid uid, string address)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_deregister_device";
			NetworkPayload deregisterPayload = networkPayload;
			this._deviceNet.QueuePacket(uid, address, deregisterPayload, null, null);
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x000D5034 File Offset: 0x000D3234
		[NullableContext(2)]
		public void Sync(EntityUid uid, string address)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "atmos_sync_data";
			NetworkPayload syncPayload = networkPayload;
			this._deviceNet.QueuePacket(uid, address, syncPayload, null, null);
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x000D5070 File Offset: 0x000D3270
		public void SetDeviceState(EntityUid uid, string address, IAtmosDeviceData data)
		{
			NetworkPayload networkPayload = new NetworkPayload();
			networkPayload["command"] = "set_state";
			networkPayload["set_state"] = data;
			NetworkPayload payload = networkPayload;
			this._deviceNet.QueuePacket(uid, address, payload, null, null);
		}

		// Token: 0x04001953 RID: 6483
		public const string RegisterDevice = "atmos_register_device";

		// Token: 0x04001954 RID: 6484
		public const string DeregisterDevice = "atmos_deregister_device";

		// Token: 0x04001955 RID: 6485
		public const string SyncData = "atmos_sync_data";

		// Token: 0x04001956 RID: 6486
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNet;
	}
}
