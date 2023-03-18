using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000050 RID: 80
	[RegisterComponent]
	public sealed class ChargeBatteryArtifactComponent : Component
	{
		// Token: 0x040000B7 RID: 183
		[DataField("radius", false, 1, false, false, null)]
		public float Radius = 15f;
	}
}
