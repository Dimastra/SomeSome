using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Temperature.Components
{
	// Token: 0x02000126 RID: 294
	[RegisterComponent]
	public sealed class ContainerTemperatureDamageThresholdsComponent : Component
	{
		// Token: 0x04000336 RID: 822
		[DataField("heatDamageThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float? HeatDamageThreshold;

		// Token: 0x04000337 RID: 823
		[DataField("coldDamageThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float? ColdDamageThreshold;
	}
}
