using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Implants.Components
{
	// Token: 0x020003F7 RID: 1015
	[RegisterComponent]
	public sealed class SubdermalImplantComponent : Component
	{
		// Token: 0x04000BDF RID: 3039
		[Nullable(2)]
		[ViewVariables]
		[DataField("implantAction", false, 1, false, false, null)]
		public string ImplantAction;

		// Token: 0x04000BE0 RID: 3040
		[ViewVariables]
		public EntityUid? ImplantedEntity;

		// Token: 0x04000BE1 RID: 3041
		[ViewVariables]
		[DataField("permanent", false, 1, false, false, null)]
		public bool Permanent;
	}
}
