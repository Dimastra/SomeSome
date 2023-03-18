using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000031 RID: 49
	[RegisterComponent]
	public sealed class ArtifactDeathTriggerComponent : Component
	{
		// Token: 0x0400007A RID: 122
		[DataField("range", false, 1, false, false, null)]
		public float Range = 15f;
	}
}
