using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D7 RID: 1751
	[NetSerializable]
	[Serializable]
	public sealed class AirAlarmTabSetMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001549 RID: 5449 RVA: 0x00045CC0 File Offset: 0x00043EC0
		public AirAlarmTabSetMessage(AirAlarmTab tab)
		{
			this.Tab = tab;
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x00045CCF File Offset: 0x00043ECF
		public AirAlarmTab Tab { get; }
	}
}
