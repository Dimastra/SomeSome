using System;
using System.Runtime.CompilerServices;
using Content.Shared.Anomaly;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Anomaly.Components
{
	// Token: 0x020007C8 RID: 1992
	[RegisterComponent]
	public sealed class AnomalousParticleComponent : Component
	{
		// Token: 0x04001ACA RID: 6858
		[DataField("particleType", false, 1, true, false, null)]
		public AnomalousParticleType ParticleType;

		// Token: 0x04001ACB RID: 6859
		[Nullable(1)]
		[DataField("fixtureId", false, 1, false, false, null)]
		public string FixtureId = "projectile";
	}
}
