using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x0200042E RID: 1070
	public sealed class VirtualItemDeletedEvent : EntityEventArgs
	{
		// Token: 0x06000CD7 RID: 3287 RVA: 0x0002A4D6 File Offset: 0x000286D6
		public VirtualItemDeletedEvent(EntityUid blockingEntity, EntityUid user)
		{
			this.BlockingEntity = blockingEntity;
			this.User = user;
		}

		// Token: 0x04000CA3 RID: 3235
		public EntityUid BlockingEntity;

		// Token: 0x04000CA4 RID: 3236
		public EntityUid User;
	}
}
