using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee
{
	// Token: 0x0200006E RID: 110
	public sealed class GetMeleeWeaponEvent : HandledEntityEventArgs
	{
		// Token: 0x0400016C RID: 364
		public EntityUid? Weapon;
	}
}
