using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory
{
	// Token: 0x020003AD RID: 941
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InventoryRelayedEvent<[Nullable(0)] TEvent> : EntityEventArgs where TEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x06000AE6 RID: 2790 RVA: 0x000242EE File Offset: 0x000224EE
		public InventoryRelayedEvent(TEvent args)
		{
			this.Args = args;
		}

		// Token: 0x04000ABD RID: 2749
		public readonly TEvent Args;
	}
}
