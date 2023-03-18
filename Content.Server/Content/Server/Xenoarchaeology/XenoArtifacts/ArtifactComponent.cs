using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts
{
	// Token: 0x0200001C RID: 28
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ArtifactComponent : Component
	{
		// Token: 0x04000041 RID: 65
		[ViewVariables]
		public ArtifactTree NodeTree;

		// Token: 0x04000042 RID: 66
		[ViewVariables]
		public ArtifactNode CurrentNode;

		// Token: 0x04000043 RID: 67
		[DataField("nodesMin", false, 1, false, false, null)]
		public int NodesMin = 3;

		// Token: 0x04000044 RID: 68
		[DataField("nodesMax", false, 1, false, false, null)]
		public int NodesMax = 9;

		// Token: 0x04000045 RID: 69
		[DataField("timer", false, 1, false, false, typeof(TimespanSerializer))]
		[ViewVariables]
		public TimeSpan CooldownTime = TimeSpan.FromSeconds(5.0);

		// Token: 0x04000046 RID: 70
		[ViewVariables]
		public bool IsSuppressed;

		// Token: 0x04000047 RID: 71
		[DataField("lastActivationTime", false, 1, false, false, typeof(TimespanSerializer))]
		public TimeSpan LastActivationTime;

		// Token: 0x04000048 RID: 72
		[DataField("pricePerNode", false, 1, false, false, null)]
		public int PricePerNode = 500;

		// Token: 0x04000049 RID: 73
		[DataField("pointsPerNode", false, 1, false, false, null)]
		public int PointsPerNode = 5000;

		// Token: 0x0400004A RID: 74
		[DataField("pointDangerMultiplier", false, 1, false, false, null)]
		public float PointDangerMultiplier = 1.35f;
	}
}
