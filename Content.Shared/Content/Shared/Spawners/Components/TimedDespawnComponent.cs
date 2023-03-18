using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Spawners.Components
{
	// Token: 0x02000184 RID: 388
	[RegisterComponent]
	public sealed class TimedDespawnComponent : Component
	{
		// Token: 0x04000451 RID: 1105
		[DataField("lifetime", false, 1, false, false, null)]
		public float Lifetime = 5f;
	}
}
