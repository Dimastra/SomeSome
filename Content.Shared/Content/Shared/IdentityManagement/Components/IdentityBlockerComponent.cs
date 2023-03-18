using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.IdentityManagement.Components
{
	// Token: 0x020003FE RID: 1022
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class IdentityBlockerComponent : Component
	{
		// Token: 0x04000BE4 RID: 3044
		public bool Enabled = true;
	}
}
