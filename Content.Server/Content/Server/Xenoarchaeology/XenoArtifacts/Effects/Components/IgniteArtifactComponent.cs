using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000056 RID: 86
	[RegisterComponent]
	public sealed class IgniteArtifactComponent : Component
	{
		// Token: 0x040000D2 RID: 210
		[DataField("range", false, 1, false, false, null)]
		public float Range = 2f;

		// Token: 0x040000D3 RID: 211
		[DataField("minFireStack", false, 1, false, false, null)]
		public int MinFireStack = 2;

		// Token: 0x040000D4 RID: 212
		[DataField("maxFireStack", false, 1, false, false, null)]
		public int MaxFireStack = 5;
	}
}
