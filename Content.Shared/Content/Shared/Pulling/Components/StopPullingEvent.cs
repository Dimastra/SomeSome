using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Pulling.Components
{
	// Token: 0x0200023E RID: 574
	public sealed class StopPullingEvent : CancellableEntityEventArgs
	{
		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x00017161 File Offset: 0x00015361
		public EntityUid? User { get; }

		// Token: 0x06000679 RID: 1657 RVA: 0x00017169 File Offset: 0x00015369
		public StopPullingEvent(EntityUid? uid = null)
		{
			this.User = uid;
		}
	}
}
