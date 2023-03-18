using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A0 RID: 416
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ContainmentFieldComponent : Component
	{
		// Token: 0x0400048F RID: 1167
		[DataField("throwForce", false, 1, false, false, null)]
		public float ThrowForce = 100f;

		// Token: 0x04000490 RID: 1168
		[DataField("maxMass", false, 1, false, false, null)]
		public float MaxMass = 10000f;
	}
}
