using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events
{
	// Token: 0x02000630 RID: 1584
	[NetSerializable]
	[Serializable]
	public sealed class CargoConsoleApproveOrderMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001321 RID: 4897 RVA: 0x0003FC75 File Offset: 0x0003DE75
		public CargoConsoleApproveOrderMessage(int orderIndex)
		{
			this.OrderIndex = orderIndex;
		}

		// Token: 0x04001312 RID: 4882
		public int OrderIndex;
	}
}
