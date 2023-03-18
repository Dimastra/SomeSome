using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Follower
{
	// Token: 0x02000479 RID: 1145
	public sealed class EntityStoppedFollowingEvent : FollowEvent
	{
		// Token: 0x06000DDA RID: 3546 RVA: 0x0002D3F4 File Offset: 0x0002B5F4
		public EntityStoppedFollowingEvent(EntityUid following, EntityUid follower) : base(following, follower)
		{
		}
	}
}
