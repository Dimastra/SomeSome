using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019C RID: 412
	public sealed class StationInitializedEvent : EntityEventArgs
	{
		// Token: 0x0600084F RID: 2127 RVA: 0x0002A96F File Offset: 0x00028B6F
		public StationInitializedEvent(EntityUid station)
		{
			this.Station = station;
		}

		// Token: 0x0400050F RID: 1295
		public EntityUid Station;
	}
}
