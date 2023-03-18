using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Teleportation.Components
{
	// Token: 0x020000E3 RID: 227
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class LinkedEntityComponentState : ComponentState
	{
		// Token: 0x06000283 RID: 643 RVA: 0x0000C212 File Offset: 0x0000A412
		public LinkedEntityComponentState(HashSet<EntityUid> linkedEntities)
		{
			this.LinkedEntities = linkedEntities;
		}

		// Token: 0x040002E6 RID: 742
		public HashSet<EntityUid> LinkedEntities;
	}
}
