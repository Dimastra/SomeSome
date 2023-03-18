using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003CC RID: 972
	public sealed class RangedInteractEvent : HandledEntityEventArgs
	{
		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000B29 RID: 2857 RVA: 0x0002472D File Offset: 0x0002292D
		public EntityUid UserUid { get; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000B2A RID: 2858 RVA: 0x00024735 File Offset: 0x00022935
		public EntityUid UsedUid { get; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000B2B RID: 2859 RVA: 0x0002473D File Offset: 0x0002293D
		public EntityUid TargetUid { get; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000B2C RID: 2860 RVA: 0x00024745 File Offset: 0x00022945
		public EntityCoordinates ClickLocation { get; }

		// Token: 0x06000B2D RID: 2861 RVA: 0x0002474D File Offset: 0x0002294D
		public RangedInteractEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
		{
			this.UserUid = user;
			this.UsedUid = used;
			this.TargetUid = target;
			this.ClickLocation = clickLocation;
		}
	}
}
