using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Blocking
{
	// Token: 0x02000671 RID: 1649
	[RegisterComponent]
	public sealed class BlockingUserComponent : Component
	{
		// Token: 0x040013DE RID: 5086
		[DataField("blockingItem", false, 1, false, false, null)]
		public EntityUid? BlockingItem;

		// Token: 0x040013DF RID: 5087
		[Nullable(1)]
		[DataField("modifiers", false, 1, false, false, null)]
		public DamageModifierSet Modifiers;

		// Token: 0x040013E0 RID: 5088
		[DataField("originalBodyType", false, 1, false, false, null)]
		public BodyType OriginalBodyType;
	}
}
