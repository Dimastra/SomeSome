using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Kitchen.Components
{
	// Token: 0x02000439 RID: 1081
	[RegisterComponent]
	public sealed class SharpComponent : Component
	{
		// Token: 0x04000DC1 RID: 3521
		[Nullable(1)]
		public HashSet<EntityUid> Butchering = new HashSet<EntityUid>();

		// Token: 0x04000DC2 RID: 3522
		[DataField("butcherDelayModifier", false, 1, false, false, null)]
		public float ButcherDelayModifier = 1f;
	}
}
