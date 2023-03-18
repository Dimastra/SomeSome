using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing
{
	// Token: 0x020000CE RID: 206
	public sealed class ThrownEvent : HandledEntityEventArgs
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000B0EA File Offset: 0x000092EA
		public EntityUid User { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600023B RID: 571 RVA: 0x0000B0F2 File Offset: 0x000092F2
		public EntityUid Thrown { get; }

		// Token: 0x0600023C RID: 572 RVA: 0x0000B0FA File Offset: 0x000092FA
		public ThrownEvent(EntityUid user, EntityUid thrown)
		{
			this.User = user;
			this.Thrown = thrown;
		}
	}
}
