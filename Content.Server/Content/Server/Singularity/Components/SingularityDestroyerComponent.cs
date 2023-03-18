using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.Components
{
	// Token: 0x020001F1 RID: 497
	[RegisterComponent]
	public sealed class SingularityDestroyerComponent : Component
	{
		// Token: 0x040005CC RID: 1484
		[DataField("active", false, 1, false, false, null)]
		[ViewVariables]
		public bool Active = true;
	}
}
