using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000542 RID: 1346
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class DamageContactsComponent : Component
	{
		// Token: 0x04000F6C RID: 3948
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier Damage = new DamageSpecifier();

		// Token: 0x04000F6D RID: 3949
		[Nullable(2)]
		[DataField("ignoreWhitelist", false, 1, false, false, null)]
		public EntityWhitelist IgnoreWhitelist;
	}
}
