using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061F RID: 1567
	public sealed class CartridgeActivatedEvent : EntityEventArgs
	{
		// Token: 0x06001308 RID: 4872 RVA: 0x0003F98C File Offset: 0x0003DB8C
		public CartridgeActivatedEvent(EntityUid loader)
		{
			this.Loader = loader;
		}

		// Token: 0x040012E5 RID: 4837
		public readonly EntityUid Loader;
	}
}
