using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002F0 RID: 752
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MovementRelayTargetComponent : Component
	{
		// Token: 0x04000885 RID: 2181
		[Nullable(1)]
		[ViewVariables]
		public readonly List<EntityUid> Entities = new List<EntityUid>();
	}
}
