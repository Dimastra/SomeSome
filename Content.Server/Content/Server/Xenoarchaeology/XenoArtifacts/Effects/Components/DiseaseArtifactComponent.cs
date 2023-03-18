using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000053 RID: 83
	[RegisterComponent]
	public sealed class DiseaseArtifactComponent : Component
	{
		// Token: 0x040000C1 RID: 193
		[Nullable(1)]
		[DataField("diseasePrototype", false, 1, false, false, typeof(PrototypeIdListSerializer<DiseasePrototype>))]
		public List<string> DiseasePrototypes = new List<string>();

		// Token: 0x040000C2 RID: 194
		[Nullable(2)]
		[ViewVariables]
		public DiseasePrototype SpawnDisease;

		// Token: 0x040000C3 RID: 195
		[DataField("range", false, 1, false, false, null)]
		[ViewVariables]
		public float Range = 5f;
	}
}
