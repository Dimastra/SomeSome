using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Strip.Components
{
	// Token: 0x0200011C RID: 284
	[RegisterComponent]
	public sealed class ThievingComponent : Component
	{
		// Token: 0x04000364 RID: 868
		[ViewVariables]
		[DataField("stripTimeReduction", false, 1, false, false, null)]
		public float StripTimeReduction = 0.5f;

		// Token: 0x04000365 RID: 869
		[ViewVariables]
		[DataField("stealthy", false, 1, false, false, null)]
		public bool Stealthy;
	}
}
