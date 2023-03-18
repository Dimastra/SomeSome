using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Systems;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005CC RID: 1484
	[Access(new Type[]
	{
		typeof(DamageOtherOnHitSystem)
	})]
	[RegisterComponent]
	public sealed class DamageOtherOnHitComponent : Component
	{
		// Token: 0x040013A7 RID: 5031
		[DataField("ignoreResistances", false, 1, false, false, null)]
		[ViewVariables]
		public bool IgnoreResistances;

		// Token: 0x040013A8 RID: 5032
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
