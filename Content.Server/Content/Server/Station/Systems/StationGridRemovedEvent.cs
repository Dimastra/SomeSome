using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Station.Systems
{
	// Token: 0x0200019E RID: 414
	public sealed class StationGridRemovedEvent : EntityEventArgs
	{
		// Token: 0x06000851 RID: 2129 RVA: 0x0002A994 File Offset: 0x00028B94
		public StationGridRemovedEvent(EntityUid gridId)
		{
			this.GridId = gridId;
		}

		// Token: 0x04000512 RID: 1298
		public EntityUid GridId;
	}
}
