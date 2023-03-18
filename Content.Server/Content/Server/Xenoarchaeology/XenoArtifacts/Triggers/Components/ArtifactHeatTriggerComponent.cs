using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000035 RID: 53
	[RegisterComponent]
	public sealed class ArtifactHeatTriggerComponent : Component
	{
		// Token: 0x0400007F RID: 127
		[DataField("activationTemperature", false, 1, false, false, null)]
		[ViewVariables]
		public float ActivationTemperature = 373f;

		// Token: 0x04000080 RID: 128
		[DataField("activateHot", false, 1, false, false, null)]
		[ViewVariables]
		public bool ActivateHotItems = true;
	}
}
