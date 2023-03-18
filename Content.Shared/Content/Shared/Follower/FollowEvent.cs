using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Follower
{
	// Token: 0x02000475 RID: 1141
	public abstract class FollowEvent : EntityEventArgs
	{
		// Token: 0x06000DD6 RID: 3542 RVA: 0x0002D3C0 File Offset: 0x0002B5C0
		protected FollowEvent(EntityUid following, EntityUid follower)
		{
			this.Following = following;
			this.Follower = follower;
		}

		// Token: 0x04000D27 RID: 3367
		public EntityUid Following;

		// Token: 0x04000D28 RID: 3368
		public EntityUid Follower;
	}
}
