using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Morgue.Components
{
	// Token: 0x02000399 RID: 921
	[RegisterComponent]
	public sealed class ActiveCrematoriumComponent : Component
	{
		// Token: 0x04000B86 RID: 2950
		[ViewVariables]
		public float Accumulator;
	}
}
