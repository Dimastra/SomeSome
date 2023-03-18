using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Armor
{
	// Token: 0x020007BA RID: 1978
	[RegisterComponent]
	public sealed class ArmorComponent : Component
	{
		// Token: 0x04001A86 RID: 6790
		[Nullable(1)]
		[DataField("modifiers", false, 1, true, false, null)]
		public DamageModifierSet Modifiers;
	}
}
