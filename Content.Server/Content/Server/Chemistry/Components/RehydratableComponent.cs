using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Chemistry.Components
{
	// Token: 0x020006AC RID: 1708
	[RegisterComponent]
	public sealed class RehydratableComponent : Component
	{
		// Token: 0x040015F6 RID: 5622
		[Nullable(1)]
		[DataField("catalyst", false, 1, false, false, null)]
		internal string CatalystPrototype = "Water";

		// Token: 0x040015F7 RID: 5623
		[Nullable(2)]
		[DataField("target", false, 1, false, false, null)]
		internal string TargetPrototype;

		// Token: 0x040015F8 RID: 5624
		internal bool Expanding;
	}
}
