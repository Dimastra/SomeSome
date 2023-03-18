using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Bed.Components
{
	// Token: 0x02000727 RID: 1831
	[RegisterComponent]
	public sealed class HealOnBuckleComponent : Component
	{
		// Token: 0x040017EF RID: 6127
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x040017F0 RID: 6128
		[DataField("healTime", false, 1, false, false, null)]
		[ViewVariables]
		public float HealTime = 1f;

		// Token: 0x040017F1 RID: 6129
		[DataField("sleepMultiplier", false, 1, false, false, null)]
		public float SleepMultiplier = 3f;

		// Token: 0x040017F2 RID: 6130
		public TimeSpan NextHealTime = TimeSpan.Zero;
	}
}
