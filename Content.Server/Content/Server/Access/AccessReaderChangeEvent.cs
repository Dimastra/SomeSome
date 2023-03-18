using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Access
{
	// Token: 0x02000877 RID: 2167
	public sealed class AccessReaderChangeEvent : EntityEventArgs
	{
		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x06002F53 RID: 12115 RVA: 0x000F4D9B File Offset: 0x000F2F9B
		public EntityUid Sender { get; }

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x06002F54 RID: 12116 RVA: 0x000F4DA3 File Offset: 0x000F2FA3
		public bool Enabled { get; }

		// Token: 0x06002F55 RID: 12117 RVA: 0x000F4DAB File Offset: 0x000F2FAB
		public AccessReaderChangeEvent(EntityUid entity, bool enabled)
		{
			this.Sender = entity;
			this.Enabled = enabled;
		}
	}
}
