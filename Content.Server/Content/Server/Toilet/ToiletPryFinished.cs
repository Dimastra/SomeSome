using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Toilet
{
	// Token: 0x0200011D RID: 285
	public sealed class ToiletPryFinished : EntityEventArgs
	{
		// Token: 0x0600052B RID: 1323 RVA: 0x000192BE File Offset: 0x000174BE
		public ToiletPryFinished(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x04000313 RID: 787
		public EntityUid Uid;
	}
}
