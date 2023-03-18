using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000034 RID: 52
	[RegisterComponent]
	public sealed class ArtifactGasTriggerComponent : Component
	{
		// Token: 0x0400007C RID: 124
		[Nullable(1)]
		[DataField("possibleGas", false, 1, false, false, null)]
		public List<Gas> PossibleGases = new List<Gas>
		{
			Gas.Oxygen,
			Gas.Plasma,
			Gas.Nitrogen,
			Gas.CarbonDioxide,
			Gas.Miasma,
			Gas.NitrousOxide
		};

		// Token: 0x0400007D RID: 125
		[DataField("gas", false, 1, false, false, null)]
		[ViewVariables]
		public Gas? ActivationGas;

		// Token: 0x0400007E RID: 126
		[DataField("moles", false, 1, false, false, null)]
		[ViewVariables]
		public float ActivationMoles = 10.392799f;
	}
}
