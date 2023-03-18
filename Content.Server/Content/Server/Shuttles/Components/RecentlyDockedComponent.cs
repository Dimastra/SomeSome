using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Shuttles.Components
{
	// Token: 0x02000208 RID: 520
	[RegisterComponent]
	public sealed class RecentlyDockedComponent : Component
	{
		// Token: 0x0400064F RID: 1615
		[DataField("lastDocked", false, 1, false, false, null)]
		public EntityUid LastDocked;

		// Token: 0x04000650 RID: 1616
		[ViewVariables]
		[DataField("radius", false, 1, false, false, null)]
		public float Radius = 1.5f;
	}
}
