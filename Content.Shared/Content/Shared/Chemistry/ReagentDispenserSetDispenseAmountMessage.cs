using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005D7 RID: 1495
	[NetSerializable]
	[Serializable]
	public sealed class ReagentDispenserSetDispenseAmountMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001207 RID: 4615 RVA: 0x0003B401 File Offset: 0x00039601
		public ReagentDispenserSetDispenseAmountMessage(ReagentDispenserDispenseAmount amount)
		{
			this.ReagentDispenserDispenseAmount = amount;
		}

		// Token: 0x040010E6 RID: 4326
		public readonly ReagentDispenserDispenseAmount ReagentDispenserDispenseAmount;
	}
}
