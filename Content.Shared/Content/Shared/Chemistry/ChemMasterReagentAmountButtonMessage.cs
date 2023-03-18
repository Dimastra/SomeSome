using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005CD RID: 1485
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChemMasterReagentAmountButtonMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001200 RID: 4608 RVA: 0x0003B312 File Offset: 0x00039512
		public ChemMasterReagentAmountButtonMessage(string reagentId, ChemMasterReagentAmount amount, bool fromBuffer)
		{
			this.ReagentId = reagentId;
			this.Amount = amount;
			this.FromBuffer = fromBuffer;
		}

		// Token: 0x040010C5 RID: 4293
		public readonly string ReagentId;

		// Token: 0x040010C6 RID: 4294
		public readonly ChemMasterReagentAmount Amount;

		// Token: 0x040010C7 RID: 4295
		public readonly bool FromBuffer;
	}
}
