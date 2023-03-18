using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events
{
	// Token: 0x0200022C RID: 556
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class OnRadiationOverlayResistanceUpdateEvent : EntityEventArgs
	{
		// Token: 0x0600062E RID: 1582 RVA: 0x00015CC3 File Offset: 0x00013EC3
		public OnRadiationOverlayResistanceUpdateEvent(Dictionary<EntityUid, Dictionary<Vector2i, float>> grids)
		{
			this.Grids = grids;
		}

		// Token: 0x04000630 RID: 1584
		public readonly Dictionary<EntityUid, Dictionary<Vector2i, float>> Grids;
	}
}
