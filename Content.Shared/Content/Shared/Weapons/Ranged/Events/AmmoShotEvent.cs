using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000053 RID: 83
	public sealed class AmmoShotEvent : EntityEventArgs
	{
		// Token: 0x040000FE RID: 254
		[Nullable(1)]
		public List<EntityUid> FiredProjectiles;
	}
}
