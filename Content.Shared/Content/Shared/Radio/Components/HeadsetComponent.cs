using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Radio.Components
{
	// Token: 0x02000225 RID: 549
	[RegisterComponent]
	public sealed class HeadsetComponent : Component
	{
		// Token: 0x0400061E RID: 1566
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled = true;

		// Token: 0x0400061F RID: 1567
		public bool IsEquipped;

		// Token: 0x04000620 RID: 1568
		[DataField("requiredSlot", false, 1, false, false, null)]
		public SlotFlags RequiredSlot = SlotFlags.EARS;
	}
}
