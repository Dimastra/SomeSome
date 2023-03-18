using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000054 RID: 84
	[RegisterComponent]
	public sealed class FoamArtifactComponent : Component
	{
		// Token: 0x040000C4 RID: 196
		[Nullable(1)]
		[DataField("reagents", false, 1, true, false, typeof(PrototypeIdListSerializer<ReagentPrototype>))]
		public List<string> Reagents = new List<string>();

		// Token: 0x040000C5 RID: 197
		[Nullable(2)]
		[ViewVariables]
		public string SelectedReagent;

		// Token: 0x040000C6 RID: 198
		[DataField("duration", false, 1, false, false, null)]
		public float Duration = 10f;

		// Token: 0x040000C7 RID: 199
		[DataField("reagentAmount", false, 1, false, false, null)]
		public float ReagentAmount = 100f;

		// Token: 0x040000C8 RID: 200
		[DataField("minFoamAmount", false, 1, false, false, null)]
		public int MinFoamAmount = 2;

		// Token: 0x040000C9 RID: 201
		[DataField("maxFoamAmount", false, 1, false, false, null)]
		public int MaxFoamAmount = 6;

		// Token: 0x040000CA RID: 202
		[DataField("spreadDuration", false, 1, false, false, null)]
		public float SpreadDuration = 1f;
	}
}
