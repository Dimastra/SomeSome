using System;
using System.Runtime.CompilerServices;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio
{
	// Token: 0x0200021D RID: 541
	public sealed class GetDefaultRadioChannelEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x00015273 File Offset: 0x00013473
		public SlotFlags TargetSlots
		{
			get
			{
				return ~SlotFlags.POCKET;
			}
		}

		// Token: 0x04000602 RID: 1538
		[Nullable(2)]
		public string Channel;
	}
}
