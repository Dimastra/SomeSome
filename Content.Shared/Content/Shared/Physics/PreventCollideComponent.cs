using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Physics
{
	// Token: 0x0200027A RID: 634
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PreventCollideComponent : Component
	{
		// Token: 0x0400074A RID: 1866
		public EntityUid Uid;
	}
}
