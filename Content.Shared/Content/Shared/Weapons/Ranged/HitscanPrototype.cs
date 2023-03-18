using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Ranged
{
	// Token: 0x02000044 RID: 68
	[NullableContext(2)]
	[Nullable(0)]
	[Prototype("hitscan", 1)]
	public sealed class HitscanPrototype : IPrototype, IShootable
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003234 File Offset: 0x00001434
		[Nullable(1)]
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { [NullableContext(1)] get; }

		// Token: 0x040000BD RID: 189
		[ViewVariables]
		[DataField("staminaDamage", false, 1, false, false, null)]
		public float StaminaDamage;

		// Token: 0x040000BE RID: 190
		[ViewVariables]
		[DataField("damage", false, 1, false, false, null)]
		public DamageSpecifier Damage;

		// Token: 0x040000BF RID: 191
		[ViewVariables]
		[DataField("muzzleFlash", false, 1, false, false, null)]
		public SpriteSpecifier MuzzleFlash;

		// Token: 0x040000C0 RID: 192
		[ViewVariables]
		[DataField("travelFlash", false, 1, false, false, null)]
		public SpriteSpecifier TravelFlash;

		// Token: 0x040000C1 RID: 193
		[ViewVariables]
		[DataField("impactFlash", false, 1, false, false, null)]
		public SpriteSpecifier ImpactFlash;

		// Token: 0x040000C2 RID: 194
		[DataField("collisionMask", false, 1, false, false, null)]
		public int CollisionMask = 1;

		// Token: 0x040000C3 RID: 195
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound;

		// Token: 0x040000C4 RID: 196
		[ViewVariables]
		[DataField("forceSound", false, 1, false, false, null)]
		public bool ForceSound;

		// Token: 0x040000C5 RID: 197
		[DataField("maxLength", false, 1, false, false, null)]
		public float MaxLength = 20f;
	}
}
