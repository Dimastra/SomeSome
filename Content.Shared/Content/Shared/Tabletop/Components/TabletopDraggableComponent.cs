using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Tabletop.Components
{
	// Token: 0x020000F3 RID: 243
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class TabletopDraggableComponent : Component
	{
		// Token: 0x04000308 RID: 776
		[ViewVariables]
		public NetUserId? DraggingPlayer;
	}
}
