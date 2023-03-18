using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Doors
{
	// Token: 0x020004E6 RID: 1254
	public sealed class DoorGetPryTimeModifierEvent : EntityEventArgs
	{
		// Token: 0x06000F26 RID: 3878 RVA: 0x00030861 File Offset: 0x0002EA61
		public DoorGetPryTimeModifierEvent(EntityUid user)
		{
			this.User = user;
		}

		// Token: 0x04000E33 RID: 3635
		public readonly EntityUid User;

		// Token: 0x04000E34 RID: 3636
		public float PryTimeModifier = 1f;
	}
}
