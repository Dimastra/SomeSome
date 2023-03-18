using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Shared.IdentityManagement.Components
{
	// Token: 0x02000400 RID: 1024
	[RegisterComponent]
	public sealed class IdentityComponent : Component
	{
		// Token: 0x04000BE5 RID: 3045
		[Nullable(1)]
		[ViewVariables]
		public ContainerSlot IdentityEntitySlot;
	}
}
