using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000079 RID: 121
	[NetSerializable]
	[Serializable]
	public sealed class StopAttackEvent : EntityEventArgs
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00008A88 File Offset: 0x00006C88
		public StopAttackEvent(EntityUid weapon)
		{
			this.Weapon = weapon;
		}

		// Token: 0x0400019A RID: 410
		public readonly EntityUid Weapon;
	}
}
