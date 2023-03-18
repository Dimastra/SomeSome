using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x0200003A RID: 58
	[RegisterComponent]
	public sealed class ArtifactMusicTriggerComponent : Component
	{
		// Token: 0x04000086 RID: 134
		[DataField("range", false, 1, false, false, null)]
		public float Range = 5f;
	}
}
