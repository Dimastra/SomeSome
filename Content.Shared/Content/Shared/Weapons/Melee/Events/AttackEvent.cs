using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000071 RID: 113
	[NetSerializable]
	[Serializable]
	public abstract class AttackEvent : EntityEventArgs
	{
		// Token: 0x0600016D RID: 365 RVA: 0x0000899F File Offset: 0x00006B9F
		protected AttackEvent(EntityCoordinates coordinates)
		{
			this.Coordinates = coordinates;
		}

		// Token: 0x04000186 RID: 390
		public readonly EntityCoordinates Coordinates;
	}
}
