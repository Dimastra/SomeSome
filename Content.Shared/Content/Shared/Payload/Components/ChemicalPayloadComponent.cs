using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Payload.Components
{
	// Token: 0x02000295 RID: 661
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ChemicalPayloadComponent : Component
	{
		// Token: 0x04000785 RID: 1925
		[DataField("beakerSlotA", false, 1, true, false, null)]
		public ItemSlot BeakerSlotA = new ItemSlot();

		// Token: 0x04000786 RID: 1926
		[DataField("beakerSlotB", false, 1, true, false, null)]
		public ItemSlot BeakerSlotB = new ItemSlot();
	}
}
