using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006B0 RID: 1712
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SolutionHeaterComponent : Component
	{
		// Token: 0x04001610 RID: 5648
		public readonly string BeakerSlotId = "beakerSlot";

		// Token: 0x04001611 RID: 5649
		[DataField("heatPerSecond", false, 1, false, false, null)]
		public float HeatPerSecond = 120f;

		// Token: 0x04001612 RID: 5650
		[ViewVariables]
		public float HeatMultiplier = 1f;

		// Token: 0x04001613 RID: 5651
		[DataField("machinePartHeatPerSecond", false, 1, false, false, null)]
		public string MachinePartHeatPerSecond = "Laser";

		// Token: 0x04001614 RID: 5652
		[DataField("partRatingHeatMultiplier", false, 1, false, false, null)]
		public float PartRatingHeatMultiplier = 1.5f;
	}
}
