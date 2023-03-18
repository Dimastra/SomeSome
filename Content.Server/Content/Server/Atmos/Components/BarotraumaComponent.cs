using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A2 RID: 1954
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BarotraumaComponent : Component
	{
		// Token: 0x04001A33 RID: 6707
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x04001A34 RID: 6708
		[DataField("maxDamage", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 MaxDamage = 200;

		// Token: 0x04001A35 RID: 6709
		public bool TakingDamage;

		// Token: 0x04001A36 RID: 6710
		[DataField("protectionSlots", false, 1, false, false, null)]
		public List<string> ProtectionSlots = new List<string>
		{
			"head",
			"outerClothing"
		};
	}
}
