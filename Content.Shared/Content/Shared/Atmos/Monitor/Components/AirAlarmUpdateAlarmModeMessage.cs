using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components
{
	// Token: 0x020006D9 RID: 1753
	[NetSerializable]
	[Serializable]
	public sealed class AirAlarmUpdateAlarmModeMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x00045CDF File Offset: 0x00043EDF
		public AirAlarmMode Mode { get; }

		// Token: 0x0600154D RID: 5453 RVA: 0x00045CE7 File Offset: 0x00043EE7
		public AirAlarmUpdateAlarmModeMessage(AirAlarmMode mode)
		{
			this.Mode = mode;
		}
	}
}
