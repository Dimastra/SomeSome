using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Fluids.Components
{
	// Token: 0x020004F8 RID: 1272
	[RegisterComponent]
	public sealed class SpillableComponent : Component
	{
		// Token: 0x0400108B RID: 4235
		[Nullable(1)]
		[DataField("solution", false, 1, false, false, null)]
		public string SolutionName = "puddle";

		// Token: 0x0400108C RID: 4236
		[DataField("spillWorn", false, 1, false, false, null)]
		public bool SpillWorn = true;

		// Token: 0x0400108D RID: 4237
		[DataField("spillDelay", false, 1, false, false, null)]
		public float? SpillDelay;
	}
}
