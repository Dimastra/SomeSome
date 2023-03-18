using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000665 RID: 1637
	public sealed class RemovedFromBodyEvent : EntityEventArgs
	{
		// Token: 0x06001418 RID: 5144 RVA: 0x000433EF File Offset: 0x000415EF
		public RemovedFromBodyEvent(EntityUid old)
		{
			this.Old = old;
		}

		// Token: 0x040013BC RID: 5052
		public EntityUid Old;
	}
}
