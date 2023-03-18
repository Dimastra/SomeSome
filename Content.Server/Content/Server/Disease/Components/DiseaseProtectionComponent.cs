using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Disease.Components
{
	// Token: 0x02000577 RID: 1399
	[RegisterComponent]
	public sealed class DiseaseProtectionComponent : Component
	{
		// Token: 0x040012DF RID: 4831
		[DataField("protection", false, 1, false, false, null)]
		public float Protection = 0.1f;

		// Token: 0x040012E0 RID: 4832
		public bool IsActive;
	}
}
