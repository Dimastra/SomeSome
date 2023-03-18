using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech
{
	// Token: 0x02000317 RID: 791
	[NetSerializable]
	[Serializable]
	public abstract class MechEquipmentUiMessage : BoundUserInterfaceMessage
	{
		// Token: 0x04000909 RID: 2313
		public EntityUid Equipment;
	}
}
