using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Mech
{
	// Token: 0x02000318 RID: 792
	[NetSerializable]
	[Serializable]
	public sealed class MechGrabberEjectMessage : MechEquipmentUiMessage
	{
		// Token: 0x06000915 RID: 2325 RVA: 0x0001E801 File Offset: 0x0001CA01
		public MechGrabberEjectMessage(EntityUid equipment, EntityUid uid)
		{
			this.Equipment = equipment;
			this.Item = uid;
		}

		// Token: 0x0400090A RID: 2314
		public EntityUid Item;
	}
}
