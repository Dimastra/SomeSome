using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C8 RID: 968
	public sealed class InteractNoHandEvent : HandledEntityEventArgs
	{
		// Token: 0x06000B20 RID: 2848 RVA: 0x000246AE File Offset: 0x000228AE
		public InteractNoHandEvent(EntityUid user, EntityUid? target, EntityCoordinates clickLocation)
		{
			this.User = user;
			this.Target = target;
			this.ClickLocation = clickLocation;
		}

		// Token: 0x04000B0D RID: 2829
		public EntityUid User;

		// Token: 0x04000B0E RID: 2830
		public EntityUid? Target;

		// Token: 0x04000B0F RID: 2831
		public EntityCoordinates ClickLocation;
	}
}
