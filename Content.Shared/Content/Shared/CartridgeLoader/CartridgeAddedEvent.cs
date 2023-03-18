using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061D RID: 1565
	public sealed class CartridgeAddedEvent : EntityEventArgs
	{
		// Token: 0x06001306 RID: 4870 RVA: 0x0003F96E File Offset: 0x0003DB6E
		public CartridgeAddedEvent(EntityUid loader)
		{
			this.Loader = loader;
		}

		// Token: 0x040012E3 RID: 4835
		public readonly EntityUid Loader;
	}
}
