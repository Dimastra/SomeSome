using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E6 RID: 230
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class PortalTimeoutComponent : Component
	{
		// Token: 0x040002EC RID: 748
		[ViewVariables]
		[DataField("enteredPortal", false, 1, false, false, null)]
		public EntityUid? EnteredPortal;
	}
}
