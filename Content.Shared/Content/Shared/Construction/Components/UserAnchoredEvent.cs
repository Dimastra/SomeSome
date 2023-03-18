using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058A RID: 1418
	public sealed class UserAnchoredEvent : BaseAnchoredEvent
	{
		// Token: 0x06001169 RID: 4457 RVA: 0x00039149 File Offset: 0x00037349
		public UserAnchoredEvent(EntityUid user, EntityUid tool) : base(user, tool)
		{
		}
	}
}
