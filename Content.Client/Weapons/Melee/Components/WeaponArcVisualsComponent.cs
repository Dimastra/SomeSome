using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Weapons.Melee.Components
{
	// Token: 0x0200003F RID: 63
	[RegisterComponent]
	public sealed class WeaponArcVisualsComponent : Component
	{
		// Token: 0x040000C7 RID: 199
		[DataField("animation", false, 1, false, false, null)]
		public WeaponArcAnimation Animation;

		// Token: 0x040000C8 RID: 200
		[ViewVariables]
		[DataField("fadeOut", false, 1, false, false, null)]
		public bool Fadeout = true;
	}
}
