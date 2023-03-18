using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x02000616 RID: 1558
	[NetSerializable]
	[Serializable]
	public sealed class CartridgeLoaderUiMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060012F9 RID: 4857 RVA: 0x0003F787 File Offset: 0x0003D987
		public CartridgeLoaderUiMessage(EntityUid cartridgeUid, CartridgeUiMessageAction action)
		{
			this.CartridgeUid = cartridgeUid;
			this.Action = action;
		}

		// Token: 0x040012D3 RID: 4819
		public readonly EntityUid CartridgeUid;

		// Token: 0x040012D4 RID: 4820
		public readonly CartridgeUiMessageAction Action;
	}
}
