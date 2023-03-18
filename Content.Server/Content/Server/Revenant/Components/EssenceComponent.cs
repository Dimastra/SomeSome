using System;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Revenant.Components
{
	// Token: 0x02000234 RID: 564
	[RegisterComponent]
	public sealed class EssenceComponent : Component
	{
		// Token: 0x040006ED RID: 1773
		[ViewVariables]
		public bool Harvested;

		// Token: 0x040006EE RID: 1774
		[ViewVariables]
		public bool SearchComplete;

		// Token: 0x040006EF RID: 1775
		[ViewVariables]
		public float EssenceAmount;
	}
}
