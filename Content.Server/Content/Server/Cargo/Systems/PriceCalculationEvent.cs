using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Cargo.Systems
{
	// Token: 0x020006E3 RID: 1763
	[ByRefEvent]
	public struct PriceCalculationEvent
	{
		// Token: 0x0600250D RID: 9485 RVA: 0x000C1FB0 File Offset: 0x000C01B0
		public PriceCalculationEvent()
		{
			this.Price = 0.0;
		}

		// Token: 0x040016B6 RID: 5814
		public double Price;
	}
}
