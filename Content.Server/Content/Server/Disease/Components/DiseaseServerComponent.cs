using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Components
{
	// Token: 0x0200057B RID: 1403
	[RegisterComponent]
	public sealed class DiseaseServerComponent : Component
	{
		// Token: 0x040012E7 RID: 4839
		[Nullable(1)]
		[ViewVariables]
		public List<DiseasePrototype> Diseases = new List<DiseasePrototype>();
	}
}
