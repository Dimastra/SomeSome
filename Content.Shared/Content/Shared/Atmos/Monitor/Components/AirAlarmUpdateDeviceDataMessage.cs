using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006DA RID: 1754
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AirAlarmUpdateDeviceDataMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x0600154E RID: 5454 RVA: 0x00045CF6 File Offset: 0x00043EF6
		public string Address { get; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x00045CFE File Offset: 0x00043EFE
		public IAtmosDeviceData Data { get; }

		// Token: 0x06001550 RID: 5456 RVA: 0x00045D06 File Offset: 0x00043F06
		public AirAlarmUpdateDeviceDataMessage(string addr, IAtmosDeviceData data)
		{
			this.Address = addr;
			this.Data = data;
		}
	}
}
