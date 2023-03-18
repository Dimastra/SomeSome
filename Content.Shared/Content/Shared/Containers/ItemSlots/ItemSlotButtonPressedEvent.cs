using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Containers.ItemSlots
{
	// Token: 0x02000561 RID: 1377
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ItemSlotButtonPressedEvent : BoundUserInterfaceMessage
	{
		// Token: 0x060010AD RID: 4269 RVA: 0x0003644A File Offset: 0x0003464A
		public ItemSlotButtonPressedEvent(string slotId, bool tryEject = true, bool tryInsert = true)
		{
			this.SlotId = slotId;
			this.TryEject = tryEject;
			this.TryInsert = tryInsert;
		}

		// Token: 0x04000FAA RID: 4010
		public string SlotId;

		// Token: 0x04000FAB RID: 4011
		public bool TryInsert;

		// Token: 0x04000FAC RID: 4012
		public bool TryEject;
	}
}
