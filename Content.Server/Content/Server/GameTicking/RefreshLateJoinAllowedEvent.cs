using System;

namespace Content.Server.GameTicking
{
	// Token: 0x020004AF RID: 1199
	public sealed class RefreshLateJoinAllowedEvent
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x0600186A RID: 6250 RVA: 0x0007F7BD File Offset: 0x0007D9BD
		// (set) Token: 0x0600186B RID: 6251 RVA: 0x0007F7C5 File Offset: 0x0007D9C5
		public bool DisallowLateJoin { get; private set; }

		// Token: 0x0600186C RID: 6252 RVA: 0x0007F7CE File Offset: 0x0007D9CE
		public void Disallow()
		{
			this.DisallowLateJoin = true;
		}
	}
}
