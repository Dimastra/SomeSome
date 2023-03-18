using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events
{
	// Token: 0x020002E1 RID: 737
	[ByRefEvent]
	public readonly struct MoveInputEvent
	{
		// Token: 0x06000853 RID: 2131 RVA: 0x0001C878 File Offset: 0x0001AA78
		public MoveInputEvent(EntityUid entity)
		{
			this.Entity = entity;
		}

		// Token: 0x04000866 RID: 2150
		public readonly EntityUid Entity;
	}
}
