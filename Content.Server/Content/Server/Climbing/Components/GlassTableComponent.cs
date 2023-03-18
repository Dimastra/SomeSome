using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Climbing.Components
{
	// Token: 0x02000649 RID: 1609
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ClimbSystem)
	})]
	public sealed class GlassTableComponent : Component
	{
		// Token: 0x04001516 RID: 5398
		[DataField("climberDamage", false, 1, false, false, null)]
		public DamageSpecifier ClimberDamage;

		// Token: 0x04001517 RID: 5399
		[DataField("tableDamage", false, 1, false, false, null)]
		public DamageSpecifier TableDamage;

		// Token: 0x04001518 RID: 5400
		[DataField("tableMassLimit", false, 1, false, false, null)]
		public float MassLimit;

		// Token: 0x04001519 RID: 5401
		public float StunTime = 2f;
	}
}
