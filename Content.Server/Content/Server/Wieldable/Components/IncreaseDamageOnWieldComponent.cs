using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Wieldable.Components
{
	// Token: 0x0200007D RID: 125
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(WieldableSystem)
	})]
	public sealed class IncreaseDamageOnWieldComponent : Component
	{
		// Token: 0x0400014D RID: 333
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier BonusDamage;
	}
}
