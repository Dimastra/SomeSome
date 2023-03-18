using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005DD RID: 1501
	[NetSerializable]
	[Serializable]
	public sealed class TransferAmountBoundInterfaceState : BoundUserInterfaceState
	{
		// Token: 0x0600120B RID: 4619 RVA: 0x0003B444 File Offset: 0x00039644
		public TransferAmountBoundInterfaceState(FixedPoint2 max, FixedPoint2 min)
		{
			this.Max = max;
			this.Min = min;
		}

		// Token: 0x040010F7 RID: 4343
		public FixedPoint2 Max;

		// Token: 0x040010F8 RID: 4344
		public FixedPoint2 Min;
	}
}
