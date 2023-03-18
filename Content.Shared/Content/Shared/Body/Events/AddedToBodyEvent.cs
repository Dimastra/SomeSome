using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000662 RID: 1634
	public sealed class AddedToBodyEvent : EntityEventArgs
	{
		// Token: 0x06001415 RID: 5141 RVA: 0x000433BB File Offset: 0x000415BB
		public AddedToBodyEvent(EntityUid body)
		{
			this.Body = body;
		}

		// Token: 0x040013B8 RID: 5048
		public EntityUid Body;
	}
}
