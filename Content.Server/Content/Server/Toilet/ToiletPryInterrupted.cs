using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Toilet
{
	// Token: 0x0200011E RID: 286
	public sealed class ToiletPryInterrupted : EntityEventArgs
	{
		// Token: 0x0600052C RID: 1324 RVA: 0x000192CD File Offset: 0x000174CD
		public ToiletPryInterrupted(EntityUid uid)
		{
			this.Uid = uid;
		}

		// Token: 0x04000314 RID: 788
		public EntityUid Uid;
	}
}
