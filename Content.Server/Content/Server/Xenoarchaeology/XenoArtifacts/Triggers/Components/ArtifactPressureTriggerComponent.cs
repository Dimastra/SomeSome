using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x0200003B RID: 59
	[RegisterComponent]
	public sealed class ArtifactPressureTriggerComponent : Component
	{
		// Token: 0x04000087 RID: 135
		[DataField("minPressureThreshold", false, 1, false, false, null)]
		public float? MinPressureThreshold;

		// Token: 0x04000088 RID: 136
		[DataField("maxPressureThreshold", false, 1, false, false, null)]
		public float? MaxPressureThreshold;
	}
}
