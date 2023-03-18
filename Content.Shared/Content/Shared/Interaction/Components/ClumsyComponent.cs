using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Interaction.Components
{
	// Token: 0x020003DE RID: 990
	[RegisterComponent]
	public sealed class ClumsyComponent : Component
	{
		// Token: 0x04000B51 RID: 2897
		[Nullable(1)]
		[DataField("clumsyDamage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier ClumsyDamage;
	}
}
