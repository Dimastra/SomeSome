using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Strip.Components
{
	// Token: 0x02000114 RID: 276
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class StrippableComponent : Component
	{
		// Token: 0x0400035A RID: 858
		[ViewVariables]
		[DataField("handDelay", false, 1, false, false, null)]
		public float HandStripDelay = 4f;
	}
}
