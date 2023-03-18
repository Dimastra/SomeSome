using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Sticky.Components
{
	// Token: 0x02000147 RID: 327
	[RegisterComponent]
	public sealed class StickyVisualizerComponent : Component
	{
		// Token: 0x040003CC RID: 972
		[DataField("stuckDrawDepth", false, 1, false, false, null)]
		[ViewVariables]
		public int StuckDrawDepth = 6;

		// Token: 0x040003CD RID: 973
		[ViewVariables]
		public int DefaultDrawDepth;
	}
}
