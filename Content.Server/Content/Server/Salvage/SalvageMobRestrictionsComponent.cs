using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Salvage
{
	// Token: 0x0200021E RID: 542
	[RegisterComponent]
	public sealed class SalvageMobRestrictionsComponent : Component
	{
		// Token: 0x0400069E RID: 1694
		[ViewVariables]
		[DataField("linkedGridEntity", false, 1, false, false, null)]
		public EntityUid LinkedGridEntity = EntityUid.Invalid;
	}
}
