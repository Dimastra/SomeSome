using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CA RID: 970
	public sealed class InteractUsingEvent : HandledEntityEventArgs
	{
		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000B22 RID: 2850 RVA: 0x000246E8 File Offset: 0x000228E8
		public EntityUid User { get; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000B23 RID: 2851 RVA: 0x000246F0 File Offset: 0x000228F0
		public EntityUid Used { get; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000B24 RID: 2852 RVA: 0x000246F8 File Offset: 0x000228F8
		public EntityUid Target { get; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000B25 RID: 2853 RVA: 0x00024700 File Offset: 0x00022900
		public EntityCoordinates ClickLocation { get; }

		// Token: 0x06000B26 RID: 2854 RVA: 0x00024708 File Offset: 0x00022908
		public InteractUsingEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
		{
			this.User = user;
			this.Used = used;
			this.Target = target;
			this.ClickLocation = clickLocation;
		}
	}
}
