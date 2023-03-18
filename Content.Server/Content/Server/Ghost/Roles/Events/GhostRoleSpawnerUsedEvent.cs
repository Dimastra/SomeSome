using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Ghost.Roles.Events
{
	// Token: 0x0200049A RID: 1178
	public sealed class GhostRoleSpawnerUsedEvent : EntityEventArgs
	{
		// Token: 0x060017B2 RID: 6066 RVA: 0x0007C054 File Offset: 0x0007A254
		public GhostRoleSpawnerUsedEvent(EntityUid spawner, EntityUid spawned)
		{
			this.Spawner = spawner;
			this.Spawned = spawned;
		}

		// Token: 0x04000EAD RID: 3757
		public EntityUid Spawner;

		// Token: 0x04000EAE RID: 3758
		public EntityUid Spawned;
	}
}
