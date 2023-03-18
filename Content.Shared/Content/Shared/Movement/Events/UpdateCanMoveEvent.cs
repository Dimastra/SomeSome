using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events
{
	// Token: 0x020002E4 RID: 740
	public sealed class UpdateCanMoveEvent : CancellableEntityEventArgs
	{
		// Token: 0x06000856 RID: 2134 RVA: 0x0001C892 File Offset: 0x0001AA92
		public UpdateCanMoveEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x0001C8A1 File Offset: 0x0001AAA1
		public EntityUid Uid { get; }
	}
}
