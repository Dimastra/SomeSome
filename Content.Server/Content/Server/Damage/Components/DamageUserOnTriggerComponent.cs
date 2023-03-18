using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005CF RID: 1487
	[RegisterComponent]
	public sealed class DamageUserOnTriggerComponent : Component
	{
		// Token: 0x040013AF RID: 5039
		[DataField("ignoreResistances", false, 1, false, false, null)]
		public bool IgnoreResistances;

		// Token: 0x040013B0 RID: 5040
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier Damage;
	}
}
