using System;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Anomaly.Effects.Components
{
	// Token: 0x0200070A RID: 1802
	[RegisterComponent]
	public sealed class PyroclasticAnomalyComponent : Component
	{
		// Token: 0x040015F6 RID: 5622
		[DataField("heatPerSecond", false, 1, false, false, null)]
		public float HeatPerSecond = 25f;

		// Token: 0x040015F7 RID: 5623
		[DataField("maximumIgnitionRadius", false, 1, false, false, null)]
		public float MaximumIgnitionRadius = 8f;

		// Token: 0x040015F8 RID: 5624
		[DataField("anomalyHotspotThreshold", false, 1, false, false, null)]
		public float AnomalyHotspotThreshold = 0.6f;

		// Token: 0x040015F9 RID: 5625
		[DataField("hotspotExposeTemperature", false, 1, false, false, null)]
		public float HotspotExposeTemperature = 1000f;

		// Token: 0x040015FA RID: 5626
		[DataField("hotspotExposeVolume", false, 1, false, false, null)]
		public float HotspotExposeVolume = 50f;

		// Token: 0x040015FB RID: 5627
		[DataField("supercriticalGas", false, 1, false, false, null)]
		public Gas SupercriticalGas = Gas.Plasma;

		// Token: 0x040015FC RID: 5628
		[DataField("supercriticalMoleAmount", false, 1, false, false, null)]
		public float SupercriticalMoleAmount = 75f;
	}
}
