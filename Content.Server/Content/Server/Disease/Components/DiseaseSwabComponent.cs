using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Components
{
	// Token: 0x02000578 RID: 1400
	[RegisterComponent]
	public sealed class DiseaseSwabComponent : Component
	{
		// Token: 0x040012E1 RID: 4833
		[DataField("swabDelay", false, 1, false, false, null)]
		public float SwabDelay = 2f;

		// Token: 0x040012E2 RID: 4834
		public bool Used;

		// Token: 0x040012E3 RID: 4835
		[Nullable(2)]
		[ViewVariables]
		public DiseasePrototype Disease;
	}
}
