using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C2 RID: 962
	public abstract class InteractEvent : HandledEntityEventArgs
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x0002459A File Offset: 0x0002279A
		public EntityUid User { get; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000B0D RID: 2829 RVA: 0x000245A2 File Offset: 0x000227A2
		public EntityUid Used { get; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x000245AA File Offset: 0x000227AA
		public EntityUid? Target { get; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000B0F RID: 2831 RVA: 0x000245B2 File Offset: 0x000227B2
		public EntityCoordinates ClickLocation { get; }

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000B10 RID: 2832 RVA: 0x000245BA File Offset: 0x000227BA
		public bool CanReach { get; }

		// Token: 0x06000B11 RID: 2833 RVA: 0x000245C2 File Offset: 0x000227C2
		public InteractEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
		{
			this.User = user;
			this.Used = used;
			this.Target = target;
			this.ClickLocation = clickLocation;
			this.CanReach = canReach;
		}
	}
}
