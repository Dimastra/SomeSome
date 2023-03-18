using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Coordinates
{
	// Token: 0x020005E2 RID: 1506
	[RegisterComponent]
	public sealed class SpawnRandomOffsetComponent : Component
	{
		// Token: 0x040013FA RID: 5114
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public float Offset = 0.5f;
	}
}
