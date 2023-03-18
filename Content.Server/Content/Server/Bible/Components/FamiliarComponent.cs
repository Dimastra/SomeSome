using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Bible.Components
{
	// Token: 0x02000722 RID: 1826
	[RegisterComponent]
	public sealed class FamiliarComponent : Component
	{
		// Token: 0x040017DA RID: 6106
		[ViewVariables]
		public EntityUid? Source;
	}
}
