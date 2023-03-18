using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000058 RID: 88
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TakeAmmoEvent : EntityEventArgs
	{
		// Token: 0x0600012D RID: 301 RVA: 0x00006D34 File Offset: 0x00004F34
		public TakeAmmoEvent(int shots, List<IShootable> ammo, EntityCoordinates coordinates, EntityUid? user)
		{
			this.Shots = shots;
			this.Ammo = ammo;
			this.Coordinates = coordinates;
			this.User = user;
		}

		// Token: 0x04000107 RID: 263
		public EntityUid? User;

		// Token: 0x04000108 RID: 264
		public readonly int Shots;

		// Token: 0x04000109 RID: 265
		public List<IShootable> Ammo;

		// Token: 0x0400010A RID: 266
		public EntityCoordinates Coordinates;
	}
}
