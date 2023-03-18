using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Fluids.EntitySystems
{
	// Token: 0x020004F3 RID: 1267
	public sealed class SprayAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x06001A22 RID: 6690 RVA: 0x0008A004 File Offset: 0x00088204
		public SprayAttemptEvent(EntityUid user)
		{
			this.User = user;
		}

		// Token: 0x04001073 RID: 4211
		public EntityUid User;
	}
}
