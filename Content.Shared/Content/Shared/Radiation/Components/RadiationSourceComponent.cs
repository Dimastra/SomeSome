using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radiation.Components
{
	// Token: 0x02000233 RID: 563
	[RegisterComponent]
	public sealed class RadiationSourceComponent : Component
	{
		// Token: 0x0400064D RID: 1613
		[ViewVariables]
		[DataField("intensity", false, 1, false, false, null)]
		public float Intensity = 1f;

		// Token: 0x0400064E RID: 1614
		[ViewVariables]
		[DataField("slope", false, 1, false, false, null)]
		public float Slope = 0.5f;
	}
}
