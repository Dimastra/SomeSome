using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Standing
{
	// Token: 0x020001A7 RID: 423
	public sealed class FellDownEvent : EntityEventArgs
	{
		// Token: 0x1700016A RID: 362
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x0002AC00 File Offset: 0x00028E00
		public EntityUid Uid { get; }

		// Token: 0x0600085E RID: 2142 RVA: 0x0002AC08 File Offset: 0x00028E08
		public FellDownEvent(EntityUid uid)
		{
			this.Uid = uid;
		}
	}
}
