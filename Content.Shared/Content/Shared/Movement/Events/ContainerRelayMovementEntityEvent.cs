using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events
{
	// Token: 0x020002E0 RID: 736
	[ByRefEvent]
	public readonly struct ContainerRelayMovementEntityEvent
	{
		// Token: 0x06000852 RID: 2130 RVA: 0x0001C86F File Offset: 0x0001AA6F
		public ContainerRelayMovementEntityEvent(EntityUid entity)
		{
			this.Entity = entity;
		}

		// Token: 0x04000865 RID: 2149
		public readonly EntityUid Entity;
	}
}
