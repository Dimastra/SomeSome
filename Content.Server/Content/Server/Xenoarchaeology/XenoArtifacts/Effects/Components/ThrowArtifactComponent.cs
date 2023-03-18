using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000060 RID: 96
	[RegisterComponent]
	public sealed class ThrowArtifactComponent : Component
	{
		// Token: 0x040000E5 RID: 229
		[DataField("range", false, 1, false, false, null)]
		public float Range = 2f;

		// Token: 0x040000E6 RID: 230
		[DataField("tilePryChance", false, 1, false, false, null)]
		public float TilePryChance = 0.5f;

		// Token: 0x040000E7 RID: 231
		[DataField("throwStrength", false, 1, false, false, null)]
		[ViewVariables]
		public float ThrowStrength = 5f;
	}
}
