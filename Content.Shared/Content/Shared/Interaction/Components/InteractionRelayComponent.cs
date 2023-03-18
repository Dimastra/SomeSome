using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Interaction.Components
{
	// Token: 0x020003DF RID: 991
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedInteractionSystem)
	})]
	public sealed class InteractionRelayComponent : Component
	{
		// Token: 0x04000B52 RID: 2898
		[ViewVariables]
		public EntityUid? RelayEntity;
	}
}
