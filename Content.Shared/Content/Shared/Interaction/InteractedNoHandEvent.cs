using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C9 RID: 969
	public sealed class InteractedNoHandEvent : HandledEntityEventArgs
	{
		// Token: 0x06000B21 RID: 2849 RVA: 0x000246CB File Offset: 0x000228CB
		public InteractedNoHandEvent(EntityUid target, EntityUid user, EntityCoordinates clickLocation)
		{
			this.Target = target;
			this.User = user;
			this.ClickLocation = clickLocation;
		}

		// Token: 0x04000B10 RID: 2832
		public EntityUid Target;

		// Token: 0x04000B11 RID: 2833
		public EntityUid User;

		// Token: 0x04000B12 RID: 2834
		public EntityCoordinates ClickLocation;
	}
}
