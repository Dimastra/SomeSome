using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000075 RID: 117
	[NetSerializable]
	[Serializable]
	public sealed class LightAttackEvent : AttackEvent
	{
		// Token: 0x06000174 RID: 372 RVA: 0x00008A03 File Offset: 0x00006C03
		public LightAttackEvent(EntityUid? target, EntityUid weapon, EntityCoordinates coordinates) : base(coordinates)
		{
			this.Target = target;
			this.Weapon = weapon;
		}

		// Token: 0x0400018C RID: 396
		public readonly EntityUid? Target;

		// Token: 0x0400018D RID: 397
		public readonly EntityUid Weapon;
	}
}
