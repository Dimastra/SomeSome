using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005CA RID: 1482
	[RegisterComponent]
	public sealed class DamageOnLandComponent : Component
	{
		// Token: 0x040013A2 RID: 5026
		[DataField("ignoreResistances", false, 1, false, false, null)]
		[ViewVariables]
		public bool IgnoreResistances;

		// Token: 0x040013A3 RID: 5027
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
