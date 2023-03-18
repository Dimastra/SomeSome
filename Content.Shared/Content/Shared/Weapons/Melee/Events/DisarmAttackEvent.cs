using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000073 RID: 115
	[NetSerializable]
	[Serializable]
	public sealed class DisarmAttackEvent : AttackEvent
	{
		// Token: 0x06000172 RID: 370 RVA: 0x000089E3 File Offset: 0x00006BE3
		public DisarmAttackEvent(EntityUid? target, EntityCoordinates coordinates) : base(coordinates)
		{
			this.Target = target;
		}

		// Token: 0x0400018A RID: 394
		public EntityUid? Target;
	}
}
