using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A3 RID: 1955
	[RegisterComponent]
	[ComponentProtoName("BreathMask")]
	public sealed class BreathToolComponent : Component
	{
		// Token: 0x04001A37 RID: 6711
		[DataField("allowedSlots", false, 1, false, false, null)]
		public SlotFlags AllowedSlots = SlotFlags.MASK;

		// Token: 0x04001A38 RID: 6712
		public bool IsFunctional;

		// Token: 0x04001A39 RID: 6713
		public EntityUid? ConnectedInternalsEntity;
	}
}
