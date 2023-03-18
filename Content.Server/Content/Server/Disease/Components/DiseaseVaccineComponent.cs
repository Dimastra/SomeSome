using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Components
{
	// Token: 0x02000579 RID: 1401
	[RegisterComponent]
	public sealed class DiseaseVaccineComponent : Component
	{
		// Token: 0x040012E4 RID: 4836
		[DataField("injectDelay", false, 1, false, false, null)]
		public float InjectDelay = 2f;

		// Token: 0x040012E5 RID: 4837
		public bool Used;

		// Token: 0x040012E6 RID: 4838
		[Nullable(2)]
		[ViewVariables]
		public DiseasePrototype Disease;
	}
}
