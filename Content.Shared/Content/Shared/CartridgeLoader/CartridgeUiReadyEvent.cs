using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000621 RID: 1569
	public sealed class CartridgeUiReadyEvent : EntityEventArgs
	{
		// Token: 0x0600130A RID: 4874 RVA: 0x0003F9AA File Offset: 0x0003DBAA
		public CartridgeUiReadyEvent(EntityUid loader)
		{
			this.Loader = loader;
		}

		// Token: 0x040012E7 RID: 4839
		public readonly EntityUid Loader;
	}
}
