using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C5 RID: 965
	public sealed class BeforeRangedInteractEvent : HandledEntityEventArgs
	{
		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000B14 RID: 2836 RVA: 0x0002460D File Offset: 0x0002280D
		public EntityUid User { get; }

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x06000B15 RID: 2837 RVA: 0x00024615 File Offset: 0x00022815
		public EntityUid Used { get; }

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0002461D File Offset: 0x0002281D
		public EntityUid? Target { get; }

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000B17 RID: 2839 RVA: 0x00024625 File Offset: 0x00022825
		public EntityCoordinates ClickLocation { get; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0002462D File Offset: 0x0002282D
		public bool CanReach { get; }

		// Token: 0x06000B19 RID: 2841 RVA: 0x00024635 File Offset: 0x00022835
		public BeforeRangedInteractEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach)
		{
			this.User = user;
			this.Used = used;
			this.Target = target;
			this.ClickLocation = clickLocation;
			this.CanReach = canReach;
		}
	}
}
