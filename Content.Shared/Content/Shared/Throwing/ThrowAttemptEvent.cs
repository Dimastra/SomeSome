using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000D1 RID: 209
	public sealed class ThrowAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000249 RID: 585 RVA: 0x0000B21B File Offset: 0x0000941B
		public ThrowAttemptEvent(EntityUid uid, EntityUid itemUid)
		{
			this.Uid = uid;
			this.ItemUid = itemUid;
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000B231 File Offset: 0x00009431
		public EntityUid Uid { get; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000B239 File Offset: 0x00009439
		public EntityUid ItemUid { get; }
	}
}
