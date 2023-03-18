using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Follower
{
	// Token: 0x02000478 RID: 1144
	public sealed class EntityStartedFollowingEvent : FollowEvent
	{
		// Token: 0x06000DD9 RID: 3545 RVA: 0x0002D3EA File Offset: 0x0002B5EA
		public EntityStartedFollowingEvent(EntityUid following, EntityUid follower) : base(following, follower)
		{
		}
	}
}
