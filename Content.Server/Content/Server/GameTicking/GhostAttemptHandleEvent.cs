using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004A5 RID: 1189
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostAttemptHandleEvent : HandledEntityEventArgs
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06001858 RID: 6232 RVA: 0x0007F6BB File Offset: 0x0007D8BB
		public Mind Mind { get; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06001859 RID: 6233 RVA: 0x0007F6C3 File Offset: 0x0007D8C3
		public bool CanReturnGlobal { get; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x0600185A RID: 6234 RVA: 0x0007F6CB File Offset: 0x0007D8CB
		// (set) Token: 0x0600185B RID: 6235 RVA: 0x0007F6D3 File Offset: 0x0007D8D3
		public bool Result { get; set; }

		// Token: 0x0600185C RID: 6236 RVA: 0x0007F6DC File Offset: 0x0007D8DC
		public GhostAttemptHandleEvent(Mind mind, bool canReturnGlobal)
		{
			this.Mind = mind;
			this.CanReturnGlobal = canReturnGlobal;
		}
	}
}
