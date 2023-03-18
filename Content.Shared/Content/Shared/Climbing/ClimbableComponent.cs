using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Climbing
{
	// Token: 0x020005C4 RID: 1476
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ClimbableComponent : Component
	{
		// Token: 0x040010B0 RID: 4272
		[DataField("range", false, 1, false, false, null)]
		public float Range = 1.0714285f;

		// Token: 0x040010B1 RID: 4273
		[DataField("delay", false, 1, false, false, null)]
		public float ClimbDelay = 0.8f;
	}
}
