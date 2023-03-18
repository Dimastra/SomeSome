using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000038 RID: 56
	[RegisterComponent]
	public sealed class ArtifactMagnetTriggerComponent : Component
	{
		// Token: 0x04000084 RID: 132
		[DataField("range", false, 1, false, false, null)]
		public float Range = 40f;

		// Token: 0x04000085 RID: 133
		[DataField("magbootRange", false, 1, false, false, null)]
		public float MagbootRange = 2f;
	}
}
