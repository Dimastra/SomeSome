using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Events
{
	// Token: 0x02000631 RID: 1585
	[NetSerializable]
	[Serializable]
	public sealed class CargoConsoleRemoveOrderMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001322 RID: 4898 RVA: 0x0003FC84 File Offset: 0x0003DE84
		public CargoConsoleRemoveOrderMessage(int orderIndex)
		{
			this.OrderIndex = orderIndex;
		}

		// Token: 0x04001313 RID: 4883
		public int OrderIndex;
	}
}
