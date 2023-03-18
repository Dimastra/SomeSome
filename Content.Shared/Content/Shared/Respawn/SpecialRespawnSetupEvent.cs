using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Respawn
{
	// Token: 0x020001FD RID: 509
	public sealed class SpecialRespawnSetupEvent : EntityEventArgs
	{
		// Token: 0x0600059F RID: 1439 RVA: 0x00014725 File Offset: 0x00012925
		public SpecialRespawnSetupEvent(EntityUid entity)
		{
			this.Entity = entity;
		}

		// Token: 0x040005C8 RID: 1480
		public EntityUid Entity;
	}
}
