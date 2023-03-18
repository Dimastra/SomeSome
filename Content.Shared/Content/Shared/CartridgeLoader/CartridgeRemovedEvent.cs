using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061E RID: 1566
	public sealed class CartridgeRemovedEvent : EntityEventArgs
	{
		// Token: 0x06001307 RID: 4871 RVA: 0x0003F97D File Offset: 0x0003DB7D
		public CartridgeRemovedEvent(EntityUid loader)
		{
			this.Loader = loader;
		}

		// Token: 0x040012E4 RID: 4836
		public readonly EntityUid Loader;
	}
}
