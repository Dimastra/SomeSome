using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.IdentityManagement.Components
{
	// Token: 0x020003FF RID: 1023
	public sealed class SeeIdentityAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00027605 File Offset: 0x00025805
		public SlotFlags TargetSlots
		{
			get
			{
				return SlotFlags.HEAD | SlotFlags.MASK;
			}
		}
	}
}
