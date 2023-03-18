using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Follower
{
	// Token: 0x02000476 RID: 1142
	public sealed class StartedFollowingEntityEvent : FollowEvent
	{
		// Token: 0x06000DD7 RID: 3543 RVA: 0x0002D3D6 File Offset: 0x0002B5D6
		public StartedFollowingEntityEvent(EntityUid following, EntityUid follower) : base(following, follower)
		{
		}
	}
}
