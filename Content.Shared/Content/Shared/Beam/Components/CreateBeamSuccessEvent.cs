using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Beam.Components
{
	// Token: 0x02000679 RID: 1657
	public sealed class CreateBeamSuccessEvent : EntityEventArgs
	{
		// Token: 0x0600144C RID: 5196 RVA: 0x00043FEB File Offset: 0x000421EB
		public CreateBeamSuccessEvent(EntityUid user, EntityUid target)
		{
			this.User = user;
			this.Target = target;
		}

		// Token: 0x040013F0 RID: 5104
		public readonly EntityUid User;

		// Token: 0x040013F1 RID: 5105
		public readonly EntityUid Target;
	}
}
