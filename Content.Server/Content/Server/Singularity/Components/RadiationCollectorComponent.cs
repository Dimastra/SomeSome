using System;
using Content.Server.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Singularity.Components
{
	// Token: 0x020001F0 RID: 496
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadiationCollectorSystem)
	})]
	public sealed class RadiationCollectorComponent : Component
	{
		// Token: 0x040005C8 RID: 1480
		[DataField("chargeModifier", false, 1, false, false, null)]
		[ViewVariables]
		public float ChargeModifier = 30000f;

		// Token: 0x040005C9 RID: 1481
		[DataField("cooldown", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan Cooldown = TimeSpan.FromSeconds(0.8100000023841858);

		// Token: 0x040005CA RID: 1482
		[ViewVariables]
		public bool Enabled;

		// Token: 0x040005CB RID: 1483
		public TimeSpan CoolDownEnd;
	}
}
