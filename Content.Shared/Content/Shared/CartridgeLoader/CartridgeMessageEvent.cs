using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader
{
	// Token: 0x0200061B RID: 1563
	[NetSerializable]
	[Serializable]
	public abstract class CartridgeMessageEvent : EntityEventArgs
	{
		// Token: 0x040012E0 RID: 4832
		public EntityUid LoaderUid;
	}
}
