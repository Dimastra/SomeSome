using System;
using Content.Shared.Radiation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radiation.Components
{
	// Token: 0x02000232 RID: 562
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RadiationPulseSystem)
	})]
	public sealed class RadiationPulseComponent : Component
	{
		// Token: 0x0400064A RID: 1610
		public TimeSpan StartTime;

		// Token: 0x0400064B RID: 1611
		public float VisualDuration = 2f;

		// Token: 0x0400064C RID: 1612
		public float VisualRange = 5f;
	}
}
