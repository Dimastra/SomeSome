using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x0200049D RID: 1181
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class VisionCorrectionComponent : Component
	{
		// Token: 0x04000D70 RID: 3440
		[ViewVariables]
		public bool IsActive;

		// Token: 0x04000D71 RID: 3441
		[DataField("visionBonus", false, 1, false, false, null)]
		public float VisionBonus = 3f;
	}
}
