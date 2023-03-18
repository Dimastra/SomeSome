using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007B0 RID: 1968
	[RegisterComponent]
	public sealed class TemperatureProtectionComponent : Component
	{
		// Token: 0x04001A82 RID: 6786
		[DataField("coefficient", false, 1, false, false, null)]
		public float Coefficient = 1f;
	}
}
