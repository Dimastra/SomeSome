using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000620 RID: 1568
	public sealed class CartridgeDeactivatedEvent : EntityEventArgs
	{
		// Token: 0x06001309 RID: 4873 RVA: 0x0003F99B File Offset: 0x0003DB9B
		public CartridgeDeactivatedEvent(EntityUid loader)
		{
			this.Loader = loader;
		}

		// Token: 0x040012E6 RID: 4838
		public readonly EntityUid Loader;
	}
}
