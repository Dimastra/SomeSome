using System;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.Monitor.Systems
{
	// Token: 0x02000780 RID: 1920
	public sealed class AtmosAlarmEvent : EntityEventArgs
	{
		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x060028DA RID: 10458 RVA: 0x000D4FA5 File Offset: 0x000D31A5
		public AtmosAlarmType AlarmType { get; }

		// Token: 0x060028DB RID: 10459 RVA: 0x000D4FAD File Offset: 0x000D31AD
		public AtmosAlarmEvent(AtmosAlarmType netMax)
		{
			this.AlarmType = netMax;
		}
	}
}
