using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x0200007A RID: 122
	[NetSerializable]
	[Serializable]
	public sealed class StopHeavyAttackEvent : EntityEventArgs
	{
		// Token: 0x06000179 RID: 377 RVA: 0x00008A97 File Offset: 0x00006C97
		public StopHeavyAttackEvent(EntityUid weapon)
		{
			this.Weapon = weapon;
		}

		// Token: 0x0400019B RID: 411
		public readonly EntityUid Weapon;
	}
}
