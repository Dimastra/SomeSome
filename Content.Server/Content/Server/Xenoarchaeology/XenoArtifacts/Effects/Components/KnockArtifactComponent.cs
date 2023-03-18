using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000057 RID: 87
	[RegisterComponent]
	public sealed class KnockArtifactComponent : Component
	{
		// Token: 0x040000D5 RID: 213
		[DataField("knockRange", false, 1, false, false, null)]
		public float KnockRange = 4f;
	}
}
