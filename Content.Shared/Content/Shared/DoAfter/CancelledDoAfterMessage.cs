using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F6 RID: 1270
	[NetSerializable]
	[Serializable]
	public sealed class CancelledDoAfterMessage : EntityEventArgs
	{
		// Token: 0x06000F5B RID: 3931 RVA: 0x000318D4 File Offset: 0x0002FAD4
		public CancelledDoAfterMessage(EntityUid uid, byte id)
		{
			this.Uid = uid;
			this.ID = id;
		}

		// Token: 0x04000E9C RID: 3740
		public EntityUid Uid;

		// Token: 0x04000E9D RID: 3741
		public byte ID;
	}
}
