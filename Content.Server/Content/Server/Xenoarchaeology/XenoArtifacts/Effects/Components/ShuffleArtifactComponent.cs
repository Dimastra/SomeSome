using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x0200005C RID: 92
	[RegisterComponent]
	public sealed class ShuffleArtifactComponent : Component
	{
		// Token: 0x040000D9 RID: 217
		[DataField("radius", false, 1, false, false, null)]
		public float Radius = 7.5f;
	}
}
