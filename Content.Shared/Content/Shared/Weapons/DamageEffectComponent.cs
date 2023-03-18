using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons
{
	// Token: 0x02000042 RID: 66
	[RegisterComponent]
	public sealed class DamageEffectComponent : Component
	{
		// Token: 0x040000B9 RID: 185
		[ViewVariables]
		public Color Color = Color.White;
	}
}
