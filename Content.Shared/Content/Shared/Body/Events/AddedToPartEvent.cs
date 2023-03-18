using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Body.Events
{
	// Token: 0x02000663 RID: 1635
	public sealed class AddedToPartEvent : EntityEventArgs
	{
		// Token: 0x06001416 RID: 5142 RVA: 0x000433CA File Offset: 0x000415CA
		public AddedToPartEvent(EntityUid part)
		{
			this.Part = part;
		}

		// Token: 0x040013B9 RID: 5049
		public EntityUid Part;
	}
}
