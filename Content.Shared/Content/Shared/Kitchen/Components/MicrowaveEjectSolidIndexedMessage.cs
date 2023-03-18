using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x02000397 RID: 919
	[NetSerializable]
	[Serializable]
	public sealed class MicrowaveEjectSolidIndexedMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A90 RID: 2704 RVA: 0x000229E7 File Offset: 0x00020BE7
		public MicrowaveEjectSolidIndexedMessage(EntityUid entityId)
		{
			this.EntityID = entityId;
		}

		// Token: 0x04000A81 RID: 2689
		public EntityUid EntityID;
	}
}
