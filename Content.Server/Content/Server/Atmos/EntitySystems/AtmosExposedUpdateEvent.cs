using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000791 RID: 1937
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public readonly struct AtmosExposedUpdateEvent
	{
		// Token: 0x06002940 RID: 10560 RVA: 0x000D6D0B File Offset: 0x000D4F0B
		public AtmosExposedUpdateEvent(EntityCoordinates coordinates, GasMixture mixture, TransformComponent transform)
		{
			this.Coordinates = coordinates;
			this.GasMixture = mixture;
			this.Transform = transform;
		}

		// Token: 0x040019A0 RID: 6560
		public readonly EntityCoordinates Coordinates;

		// Token: 0x040019A1 RID: 6561
		public readonly GasMixture GasMixture;

		// Token: 0x040019A2 RID: 6562
		public readonly TransformComponent Transform;
	}
}
