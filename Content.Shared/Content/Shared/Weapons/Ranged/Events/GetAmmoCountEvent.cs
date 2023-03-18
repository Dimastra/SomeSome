using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Events
{
	// Token: 0x02000054 RID: 84
	[ByRefEvent]
	public struct GetAmmoCountEvent
	{
		// Token: 0x040000FF RID: 255
		public int Count;

		// Token: 0x04000100 RID: 256
		public int Capacity;
	}
}
