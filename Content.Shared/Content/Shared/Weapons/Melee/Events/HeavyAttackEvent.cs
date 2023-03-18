using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000074 RID: 116
	[NetSerializable]
	[Serializable]
	public sealed class HeavyAttackEvent : AttackEvent
	{
		// Token: 0x06000173 RID: 371 RVA: 0x000089F3 File Offset: 0x00006BF3
		public HeavyAttackEvent(EntityUid weapon, EntityCoordinates coordinates) : base(coordinates)
		{
			this.Weapon = weapon;
		}

		// Token: 0x0400018B RID: 395
		public readonly EntityUid Weapon;
	}
}
