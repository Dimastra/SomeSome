using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000666 RID: 1638
	public sealed class RemovedFromPartEvent : EntityEventArgs
	{
		// Token: 0x06001419 RID: 5145 RVA: 0x000433FE File Offset: 0x000415FE
		public RemovedFromPartEvent(EntityUid old)
		{
			this.Old = old;
		}

		// Token: 0x040013BD RID: 5053
		public EntityUid Old;
	}
}
