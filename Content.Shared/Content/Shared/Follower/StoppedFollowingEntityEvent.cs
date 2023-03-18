using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Follower
{
	// Token: 0x02000477 RID: 1143
	public sealed class StoppedFollowingEntityEvent : FollowEvent
	{
		// Token: 0x06000DD8 RID: 3544 RVA: 0x0002D3E0 File Offset: 0x0002B5E0
		public StoppedFollowingEntityEvent(EntityUid following, EntityUid follower) : base(following, follower)
		{
		}
	}
}
