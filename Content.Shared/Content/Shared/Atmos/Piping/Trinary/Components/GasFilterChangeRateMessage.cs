using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components
{
	// Token: 0x020006B5 RID: 1717
	[NetSerializable]
	[Serializable]
	public sealed class GasFilterChangeRateMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060014E7 RID: 5351 RVA: 0x000451E5 File Offset: 0x000433E5
		public float Rate { get; }

		// Token: 0x060014E8 RID: 5352 RVA: 0x000451ED File Offset: 0x000433ED
		public GasFilterChangeRateMessage(float rate)
		{
			this.Rate = rate;
		}
	}
}
