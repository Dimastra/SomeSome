using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen
{
	// Token: 0x0200038D RID: 909
	[NetSerializable]
	[Serializable]
	public sealed class ReagentGrinderEjectChamberContentMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06000A89 RID: 2697 RVA: 0x0002293C File Offset: 0x00020B3C
		public ReagentGrinderEjectChamberContentMessage(EntityUid entityId)
		{
			this.EntityId = entityId;
		}

		// Token: 0x04000A6F RID: 2671
		public EntityUid EntityId;
	}
}
