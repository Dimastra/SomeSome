using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061A RID: 1562
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CartridgeUiMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060012FB RID: 4859 RVA: 0x0003F7B0 File Offset: 0x0003D9B0
		public CartridgeUiMessage(CartridgeMessageEvent messageEvent)
		{
			this.MessageEvent = messageEvent;
		}

		// Token: 0x040012DF RID: 4831
		public CartridgeMessageEvent MessageEvent;
	}
}
