using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x0200051A RID: 1306
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeviceListUpdateEvent : EntityEventArgs
	{
		// Token: 0x06000FD3 RID: 4051 RVA: 0x00033059 File Offset: 0x00031259
		public DeviceListUpdateEvent(List<EntityUid> oldDevices, List<EntityUid> devices)
		{
			this.OldDevices = oldDevices;
			this.Devices = devices;
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x0003306F File Offset: 0x0003126F
		public List<EntityUid> OldDevices { get; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x00033077 File Offset: 0x00031277
		public List<EntityUid> Devices { get; }
	}
}
