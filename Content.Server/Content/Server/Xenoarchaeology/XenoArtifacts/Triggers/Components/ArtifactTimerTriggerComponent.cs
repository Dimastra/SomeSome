using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x0200003C RID: 60
	[RegisterComponent]
	public sealed class ArtifactTimerTriggerComponent : Component
	{
		// Token: 0x04000089 RID: 137
		[DataField("rate", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan ActivationRate = TimeSpan.FromSeconds(5.0);

		// Token: 0x0400008A RID: 138
		public TimeSpan LastActivation;
	}
}
