using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000055 RID: 85
	[RegisterComponent]
	public sealed class GasArtifactComponent : Component
	{
		// Token: 0x040000CB RID: 203
		[ViewVariables]
		[DataField("spawnGas", false, 1, false, false, null)]
		public Gas? SpawnGas;

		// Token: 0x040000CC RID: 204
		[Nullable(1)]
		[DataField("possibleGas", false, 1, false, false, null)]
		public List<Gas> PossibleGases = new List<Gas>
		{
			Gas.Oxygen,
			Gas.Plasma,
			Gas.Nitrogen,
			Gas.CarbonDioxide,
			Gas.Tritium,
			Gas.Miasma,
			Gas.NitrousOxide,
			Gas.Frezon
		};

		// Token: 0x040000CD RID: 205
		[ViewVariables]
		[DataField("spawnTemperature", false, 1, false, false, null)]
		public float? SpawnTemperature;

		// Token: 0x040000CE RID: 206
		[DataField("minRandomTemp", false, 1, false, false, null)]
		public float MinRandomTemperature = 100f;

		// Token: 0x040000CF RID: 207
		[DataField("maxRandomTemp", false, 1, false, false, null)]
		public float MaxRandomTemperature = 400f;

		// Token: 0x040000D0 RID: 208
		[ViewVariables]
		[DataField("maxExternalPressure", false, 1, false, false, null)]
		public float MaxExternalPressure = 6500f;

		// Token: 0x040000D1 RID: 209
		[ViewVariables]
		[DataField("spawnAmount", false, 1, false, false, null)]
		public float SpawnAmount = 311.784f;
	}
}
