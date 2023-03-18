using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Ranged.Systems
{
	// Token: 0x0200004D RID: 77
	public abstract class SharedTetherGunSystem : EntitySystem
	{
		// Token: 0x040000F8 RID: 248
		[Nullable(1)]
		public const string CommandName = "tethergun";
	}
}
