using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Destructible.Thresholds;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible
{
	// Token: 0x02000595 RID: 1429
	[RegisterComponent]
	public sealed class DestructibleComponent : Component
	{
		// Token: 0x0400131E RID: 4894
		[Nullable(1)]
		[DataField("thresholds", false, 1, false, false, null)]
		public List<DamageThreshold> Thresholds = new List<DamageThreshold>();
	}
}
