using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006DB RID: 1755
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class AirAlarmUpdateAlarmThresholdMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x00045D1C File Offset: 0x00043F1C
		public string Address { get; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001552 RID: 5458 RVA: 0x00045D24 File Offset: 0x00043F24
		public AtmosAlarmThreshold Threshold { get; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001553 RID: 5459 RVA: 0x00045D2C File Offset: 0x00043F2C
		public AtmosMonitorThresholdType Type { get; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001554 RID: 5460 RVA: 0x00045D34 File Offset: 0x00043F34
		public Gas? Gas { get; }

		// Token: 0x06001555 RID: 5461 RVA: 0x00045D3C File Offset: 0x00043F3C
		public AirAlarmUpdateAlarmThresholdMessage(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
		{
			this.Address = address;
			this.Threshold = threshold;
			this.Type = type;
			this.Gas = gas;
		}
	}
}
