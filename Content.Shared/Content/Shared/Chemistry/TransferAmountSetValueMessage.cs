using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005DE RID: 1502
	[NetSerializable]
	[Serializable]
	public sealed class TransferAmountSetValueMessage : BoundUserInterfaceMessage
	{
		// Token: 0x0600120C RID: 4620 RVA: 0x0003B45A File Offset: 0x0003965A
		public TransferAmountSetValueMessage(FixedPoint2 value)
		{
			this.Value = value;
		}

		// Token: 0x040010F9 RID: 4345
		public FixedPoint2 Value;
	}
}
