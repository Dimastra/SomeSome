using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Abilities.Boxer
{
	// Token: 0x02000886 RID: 2182
	[RegisterComponent]
	public sealed class BoxerComponent : Component
	{
		// Token: 0x04001CA0 RID: 7328
		[Nullable(1)]
		[DataField("modifiers", false, 1, true, false, null)]
		public DamageModifierSet UnarmedModifiers;

		// Token: 0x04001CA1 RID: 7329
		[DataField("rangeBonus", false, 1, false, false, null)]
		public float RangeBonus = 1.5f;

		// Token: 0x04001CA2 RID: 7330
		[DataField("boxingGlovesModifier", false, 1, false, false, null)]
		public float BoxingGlovesModifier = 1.75f;
	}
}
