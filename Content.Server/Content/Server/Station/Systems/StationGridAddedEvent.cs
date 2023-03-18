using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019D RID: 413
	public sealed class StationGridAddedEvent : EntityEventArgs
	{
		// Token: 0x06000850 RID: 2128 RVA: 0x0002A97E File Offset: 0x00028B7E
		public StationGridAddedEvent(EntityUid gridId, bool isSetup)
		{
			this.GridId = gridId;
			this.IsSetup = isSetup;
		}

		// Token: 0x04000510 RID: 1296
		public EntityUid GridId;

		// Token: 0x04000511 RID: 1297
		public bool IsSetup;
	}
}
