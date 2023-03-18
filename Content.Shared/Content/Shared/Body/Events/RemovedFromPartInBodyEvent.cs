using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000667 RID: 1639
	public sealed class RemovedFromPartInBodyEvent : EntityEventArgs
	{
		// Token: 0x0600141A RID: 5146 RVA: 0x0004340D File Offset: 0x0004160D
		public RemovedFromPartInBodyEvent(EntityUid oldBody, EntityUid oldPart)
		{
			this.OldBody = oldBody;
			this.OldPart = oldPart;
		}

		// Token: 0x040013BE RID: 5054
		public EntityUid OldBody;

		// Token: 0x040013BF RID: 5055
		public EntityUid OldPart;
	}
}
