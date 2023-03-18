using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000072 RID: 114
	public sealed class AttackedEvent : EntityEventArgs
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000089AE File Offset: 0x00006BAE
		public EntityUid Used { get; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600016F RID: 367 RVA: 0x000089B6 File Offset: 0x00006BB6
		public EntityUid User { get; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000170 RID: 368 RVA: 0x000089BE File Offset: 0x00006BBE
		public EntityCoordinates ClickLocation { get; }

		// Token: 0x06000171 RID: 369 RVA: 0x000089C6 File Offset: 0x00006BC6
		public AttackedEvent(EntityUid used, EntityUid user, EntityCoordinates clickLocation)
		{
			this.Used = used;
			this.User = user;
			this.ClickLocation = clickLocation;
		}
	}
}
