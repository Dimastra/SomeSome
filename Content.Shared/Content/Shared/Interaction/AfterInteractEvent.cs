using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
	// Token: 0x020003C3 RID: 963
	public sealed class AfterInteractEvent : InteractEvent
	{
		// Token: 0x06000B12 RID: 2834 RVA: 0x000245EF File Offset: 0x000227EF
		public AfterInteractEvent(EntityUid user, EntityUid used, EntityUid? target, EntityCoordinates clickLocation, bool canReach) : base(user, used, target, clickLocation, canReach)
		{
		}
	}
}
