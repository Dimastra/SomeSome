using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Wieldable
{
	// Token: 0x0200007C RID: 124
	public sealed class ItemUnwieldedEvent : EntityEventArgs
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000A542 File Offset: 0x00008742
		public ItemUnwieldedEvent(EntityUid? user = null, bool force = false)
		{
			this.User = user;
			this.Force = force;
		}

		// Token: 0x0400014B RID: 331
		public EntityUid? User;

		// Token: 0x0400014C RID: 332
		public bool Force;
	}
}
