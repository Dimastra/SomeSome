using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C4 RID: 964
	public sealed class AfterInteractUsingEvent : InteractEvent
	{
		// Token: 0x06000B13 RID: 2835 RVA: 0x000245FE File Offset: 0x000227FE
		public AfterInteractUsingEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach) : base(user, used, target, clickLocation, canReach)
		{
		}
	}
}
