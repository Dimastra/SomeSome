using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mech.Components
{
	// Token: 0x02000328 RID: 808
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MechPilotComponent : Component
	{
		// Token: 0x04000925 RID: 2341
		[ViewVariables]
		public EntityUid Mech;
	}
}
