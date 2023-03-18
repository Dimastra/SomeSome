using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components
{
	// Token: 0x02000052 RID: 82
	[RegisterComponent]
	public sealed class DamageNearbyArtifactComponent : Component
	{
		// Token: 0x040000BC RID: 188
		[DataField("radius", false, 1, false, false, null)]
		public float Radius = 3f;

		// Token: 0x040000BD RID: 189
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x040000BE RID: 190
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier Damage;

		// Token: 0x040000BF RID: 191
		[DataField("damageChance", false, 1, false, false, null)]
		public float DamageChance = 1f;

		// Token: 0x040000C0 RID: 192
		[DataField("ignoreResistances", false, 1, false, false, null)]
		public bool IgnoreResistances;
	}
}
