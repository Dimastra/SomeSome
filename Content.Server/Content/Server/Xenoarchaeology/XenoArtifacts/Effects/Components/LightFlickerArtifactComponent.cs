using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000058 RID: 88
	[RegisterComponent]
	public sealed class LightFlickerArtifactComponent : Component
	{
		// Token: 0x040000D6 RID: 214
		[DataField("radius", false, 1, false, false, null)]
		public float Radius = 4f;

		// Token: 0x040000D7 RID: 215
		[DataField("flickerChance", false, 1, false, false, null)]
		public float FlickerChance = 0.75f;
	}
}
