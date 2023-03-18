using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000717 RID: 1815
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(RespiratorSystem)
	})]
	public sealed class RespiratorComponent : Component
	{
		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x0600262C RID: 9772 RVA: 0x000C976E File Offset: 0x000C796E
		// (set) Token: 0x0600262D RID: 9773 RVA: 0x000C9776 File Offset: 0x000C7976
		[DataField("gaspPopupCooldown", false, 1, false, false, null)]
		public TimeSpan GaspPopupCooldown { get; private set; } = TimeSpan.FromSeconds(8.0);

		// Token: 0x040017AD RID: 6061
		[DataField("saturation", false, 1, false, false, null)]
		public float Saturation = 5f;

		// Token: 0x040017AE RID: 6062
		[DataField("suffocationThreshold", false, 1, false, false, null)]
		public float SuffocationThreshold;

		// Token: 0x040017AF RID: 6063
		[DataField("maxSaturation", false, 1, false, false, null)]
		public float MaxSaturation = 5f;

		// Token: 0x040017B0 RID: 6064
		[DataField("minSaturation", false, 1, false, false, null)]
		public float MinSaturation = -2f;

		// Token: 0x040017B1 RID: 6065
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x040017B2 RID: 6066
		[DataField("damageRecovery", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier DamageRecovery;

		// Token: 0x040017B4 RID: 6068
		[ViewVariables]
		public TimeSpan LastGaspPopupTime;

		// Token: 0x040017B5 RID: 6069
		[ViewVariables]
		public int SuffocationCycles;

		// Token: 0x040017B6 RID: 6070
		[ViewVariables]
		public int SuffocationCycleThreshold = 3;

		// Token: 0x040017B7 RID: 6071
		[ViewVariables]
		public RespiratorStatus Status;

		// Token: 0x040017B8 RID: 6072
		[DataField("cycleDelay", false, 1, false, false, null)]
		public float CycleDelay = 2f;

		// Token: 0x040017B9 RID: 6073
		public float AccumulatedFrametime;
	}
}
