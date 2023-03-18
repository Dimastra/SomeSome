using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Teleportation.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E2 RID: 226
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(LinkedEntitySystem)
	})]
	[NetworkedComponent]
	public sealed class LinkedEntityComponent : Component
	{
		// Token: 0x040002E4 RID: 740
		[Nullable(1)]
		[DataField("linkedEntities", false, 1, false, false, null)]
		public HashSet<EntityUid> LinkedEntities = new HashSet<EntityUid>();

		// Token: 0x040002E5 RID: 741
		[DataField("deleteOnEmptyLinks", false, 1, false, false, null)]
		public bool DeleteOnEmptyLinks;
	}
}
