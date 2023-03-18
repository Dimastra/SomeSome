using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x0200005B RID: 91
	[RegisterComponent]
	public sealed class RandomTeleportArtifactComponent : Component
	{
		// Token: 0x040000D8 RID: 216
		[DataField("range", false, 1, false, false, null)]
		public float Range = 7.5f;
	}
}
