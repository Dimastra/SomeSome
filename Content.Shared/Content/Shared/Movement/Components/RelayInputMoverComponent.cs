using System;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002F2 RID: 754
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedMoverController)
	})]
	public sealed class RelayInputMoverComponent : Component
	{
		// Token: 0x0400089C RID: 2204
		[ViewVariables]
		public EntityUid? RelayEntity;
	}
}
