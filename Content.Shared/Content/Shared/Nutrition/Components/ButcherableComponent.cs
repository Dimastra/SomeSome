using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Nutrition.Components
{
	// Token: 0x020002AD RID: 685
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ButcherableComponent : Component
	{
		// Token: 0x040007BF RID: 1983
		[Nullable(1)]
		[DataField("spawned", false, 1, true, false, null)]
		public List<EntitySpawnEntry> SpawnedEntities = new List<EntitySpawnEntry>();

		// Token: 0x040007C0 RID: 1984
		[ViewVariables]
		[DataField("butcherDelay", false, 1, false, false, null)]
		public float ButcherDelay = 8f;

		// Token: 0x040007C1 RID: 1985
		[ViewVariables]
		[DataField("butcheringType", false, 1, false, false, null)]
		public ButcheringType Type;

		// Token: 0x040007C2 RID: 1986
		[ViewVariables]
		public bool BeingButchered;
	}
}
