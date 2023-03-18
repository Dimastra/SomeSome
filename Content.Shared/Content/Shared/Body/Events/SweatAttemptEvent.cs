using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000669 RID: 1641
	public sealed class SweatAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x0600141D RID: 5149 RVA: 0x0004343A File Offset: 0x0004163A
		public SweatAttemptEvent(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600141E RID: 5150 RVA: 0x00043449 File Offset: 0x00041649
		public EntityUid Uid { get; }
	}
}
