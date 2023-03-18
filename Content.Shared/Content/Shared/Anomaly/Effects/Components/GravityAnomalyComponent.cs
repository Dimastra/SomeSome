using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components
{
	// Token: 0x02000709 RID: 1801
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class GravityAnomalyComponent : Component
	{
		// Token: 0x040015EF RID: 5615
		[DataField("maxGravityWellRange", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxGravityWellRange = 8f;

		// Token: 0x040015F0 RID: 5616
		[DataField("maxThrowRange", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxThrowRange = 5f;

		// Token: 0x040015F1 RID: 5617
		[DataField("maxThrowStrength", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxThrowStrength = 10f;

		// Token: 0x040015F2 RID: 5618
		[DataField("maxRadiationIntensity", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxRadiationIntensity = 3f;

		// Token: 0x040015F3 RID: 5619
		[DataField("minAccel", false, 1, false, false, null)]
		[ViewVariables]
		public float MinAccel = 1f;

		// Token: 0x040015F4 RID: 5620
		[DataField("maxAccel", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxAccel = 5f;

		// Token: 0x040015F5 RID: 5621
		[DataField("spaceRange", false, 1, false, false, null)]
		[ViewVariables]
		public float SpaceRange = 3f;
	}
}
