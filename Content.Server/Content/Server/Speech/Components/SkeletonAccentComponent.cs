using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001CC RID: 460
	[RegisterComponent]
	public sealed class SkeletonAccentComponent : Component
	{
		// Token: 0x04000557 RID: 1367
		[DataField("ackChance", false, 1, false, false, null)]
		public float ackChance = 0.3f;
	}
}
