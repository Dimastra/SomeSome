using System;

namespace Content.Shared.Inventory
{
	// Token: 0x020003AE RID: 942
	public interface IInventoryRelayEvent
	{
		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000AE7 RID: 2791
		SlotFlags TargetSlots { get; }
	}
}
