using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech
{
	// Token: 0x02000316 RID: 790
	[NetSerializable]
	[Serializable]
	public sealed class MechEquipmentRemoveMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000913 RID: 2323 RVA: 0x0001E7EA File Offset: 0x0001C9EA
		public MechEquipmentRemoveMessage(EntityUid equipment)
		{
			this.Equipment = equipment;
		}

		// Token: 0x04000908 RID: 2312
		public EntityUid Equipment;
	}
}
