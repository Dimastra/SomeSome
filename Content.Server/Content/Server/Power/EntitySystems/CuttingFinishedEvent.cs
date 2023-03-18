using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028D RID: 653
	public sealed class CuttingFinishedEvent : EntityEventArgs
	{
		// Token: 0x06000D1E RID: 3358 RVA: 0x00044BDC File Offset: 0x00042DDC
		public CuttingFinishedEvent(EntityUid user)
		{
			this.User = user;
		}

		// Token: 0x040007EE RID: 2030
		public EntityUid User;
	}
}
