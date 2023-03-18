using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002EA RID: 746
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class JetpackUserComponent : Component
	{
		// Token: 0x0400087E RID: 2174
		public EntityUid Jetpack;
	}
}
