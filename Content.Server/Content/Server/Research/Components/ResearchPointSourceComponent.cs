using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Research.Components
{
	// Token: 0x02000243 RID: 579
	[RegisterComponent]
	public sealed class ResearchPointSourceComponent : Component
	{
		// Token: 0x0400071B RID: 1819
		[DataField("pointspersecond", false, 1, false, false, null)]
		[ViewVariables]
		public int PointsPerSecond;

		// Token: 0x0400071C RID: 1820
		[DataField("active", false, 1, false, false, null)]
		[ViewVariables]
		public bool Active;
	}
}
