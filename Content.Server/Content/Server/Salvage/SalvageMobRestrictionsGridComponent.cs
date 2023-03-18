using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Salvage
{
	// Token: 0x0200021F RID: 543
	[RegisterComponent]
	public sealed class SalvageMobRestrictionsGridComponent : Component
	{
		// Token: 0x0400069F RID: 1695
		[Nullable(1)]
		[ViewVariables]
		[DataField("mobsToKill", false, 1, false, false, null)]
		public List<EntityUid> MobsToKill = new List<EntityUid>();
	}
}
