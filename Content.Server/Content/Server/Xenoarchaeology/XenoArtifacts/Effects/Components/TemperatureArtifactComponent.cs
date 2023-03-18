using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x0200005F RID: 95
	[RegisterComponent]
	public sealed class TemperatureArtifactComponent : Component
	{
		// Token: 0x040000E1 RID: 225
		[DataField("targetTemp", false, 1, false, false, null)]
		[ViewVariables]
		public float TargetTemperature = 273.15f;

		// Token: 0x040000E2 RID: 226
		[DataField("spawnTemp", false, 1, false, false, null)]
		public float SpawnTemperature = 100f;

		// Token: 0x040000E3 RID: 227
		[DataField("maxTempDif", false, 1, false, false, null)]
		public float MaxTemperatureDifference = 1f;

		// Token: 0x040000E4 RID: 228
		[DataField("affectAdjacent", false, 1, false, false, null)]
		public bool AffectAdjacentTiles = true;
	}
}
