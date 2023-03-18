using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000664 RID: 1636
	public sealed class AddedToPartInBodyEvent : EntityEventArgs
	{
		// Token: 0x06001417 RID: 5143 RVA: 0x000433D9 File Offset: 0x000415D9
		public AddedToPartInBodyEvent(EntityUid body, EntityUid part)
		{
			this.Body = body;
			this.Part = part;
		}

		// Token: 0x040013BA RID: 5050
		public EntityUid Body;

		// Token: 0x040013BB RID: 5051
		public EntityUid Part;
	}
}
