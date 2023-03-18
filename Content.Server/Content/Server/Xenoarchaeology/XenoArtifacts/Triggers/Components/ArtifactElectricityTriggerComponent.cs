using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000032 RID: 50
	[RegisterComponent]
	public sealed class ArtifactElectricityTriggerComponent : Component
	{
		// Token: 0x0400007B RID: 123
		[DataField("minPower", false, 1, false, false, null)]
		public float MinPower = 400f;
	}
}
