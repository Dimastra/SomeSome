using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Containers.ItemSlots
{
	// Token: 0x02000563 RID: 1379
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ItemSlotsComponentState : ComponentState
	{
		// Token: 0x060010AF RID: 4271 RVA: 0x0003647A File Offset: 0x0003467A
		public ItemSlotsComponentState(Dictionary<string, ItemSlot> slots)
		{
			this.Slots = slots;
		}

		// Token: 0x04000FAE RID: 4014
		public readonly Dictionary<string, ItemSlot> Slots;
	}
}
