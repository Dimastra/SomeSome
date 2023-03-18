using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000792 RID: 1938
	[ByRefEvent]
	public struct AtmosExposedGetAirEvent
	{
		// Token: 0x06002941 RID: 10561 RVA: 0x000D6D22 File Offset: 0x000D4F22
		public AtmosExposedGetAirEvent(EntityUid entity, bool invalidate = false)
		{
			this.Gas = null;
			this.Invalidate = false;
			this.Handled = false;
			this.Entity = entity;
			this.Invalidate = invalidate;
		}

		// Token: 0x040019A3 RID: 6563
		public readonly EntityUid Entity;

		// Token: 0x040019A4 RID: 6564
		[Nullable(2)]
		public GasMixture Gas;

		// Token: 0x040019A5 RID: 6565
		public bool Invalidate;

		// Token: 0x040019A6 RID: 6566
		public bool Handled;
	}
}
