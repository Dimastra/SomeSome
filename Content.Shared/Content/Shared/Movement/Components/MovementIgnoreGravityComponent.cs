using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002ED RID: 749
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MovementIgnoreGravityComponent : Component
	{
		// Token: 0x04000883 RID: 2179
		[DataField("gravityState", false, 1, false, false, null)]
		public bool Weightless;
	}
}
