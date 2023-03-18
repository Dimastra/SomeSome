using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Slippery
{
	// Token: 0x02000199 RID: 409
	public sealed class SlipAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060004D4 RID: 1236 RVA: 0x00012946 File Offset: 0x00010B46
		public SlotFlags TargetSlots { get; } = 16384;
	}
}
