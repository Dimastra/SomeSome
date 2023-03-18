using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000078 RID: 120
	[NetSerializable]
	[Serializable]
	public sealed class StartHeavyAttackEvent : EntityEventArgs
	{
		// Token: 0x06000177 RID: 375 RVA: 0x00008A79 File Offset: 0x00006C79
		public StartHeavyAttackEvent(EntityUid weapon)
		{
			this.Weapon = weapon;
		}

		// Token: 0x04000199 RID: 409
		public readonly EntityUid Weapon;
	}
}
