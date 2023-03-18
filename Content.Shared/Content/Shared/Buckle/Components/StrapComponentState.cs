using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Buckle.Components
{
	// Token: 0x02000647 RID: 1607
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StrapComponentState : ComponentState
	{
		// Token: 0x06001376 RID: 4982 RVA: 0x000409CC File Offset: 0x0003EBCC
		public StrapComponentState(StrapPosition position, Vector2 offset, HashSet<EntityUid> buckled, float maxBuckleDistance)
		{
			this.Position = position;
			this.BuckleOffsetClamped = offset;
			this.BuckledEntities = buckled;
			this.MaxBuckleDistance = maxBuckleDistance;
		}

		// Token: 0x04001366 RID: 4966
		public StrapPosition Position;

		// Token: 0x04001367 RID: 4967
		public float MaxBuckleDistance;

		// Token: 0x04001368 RID: 4968
		public Vector2 BuckleOffsetClamped;

		// Token: 0x04001369 RID: 4969
		public HashSet<EntityUid> BuckledEntities;
	}
}
