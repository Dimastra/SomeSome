using System;
using System.Runtime.CompilerServices;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;

namespace Content.Server.CartridgeLoader
{
	// Token: 0x020006DB RID: 1755
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CartridgeAfterInteractEvent : EntityEventArgs
	{
		// Token: 0x060024AD RID: 9389 RVA: 0x000BF046 File Offset: 0x000BD246
		public CartridgeAfterInteractEvent(EntityUid loader, AfterInteractEvent interactEvent)
		{
			this.Loader = loader;
			this.InteractEvent = interactEvent;
		}

		// Token: 0x04001686 RID: 5766
		public readonly EntityUid Loader;

		// Token: 0x04001687 RID: 5767
		public readonly AfterInteractEvent InteractEvent;
	}
}
