using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Climbing
{
	// Token: 0x020005C2 RID: 1474
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(BonkSystem)
	})]
	public sealed class BonkableComponent : Component
	{
		// Token: 0x040010A6 RID: 4262
		[DataField("bonkClumsyChance", false, 1, false, false, null)]
		public float BonkClumsyChance = 0.75f;

		// Token: 0x040010A7 RID: 4263
		[DataField("bonkSound", false, 1, false, false, null)]
		public SoundSpecifier BonkSound;

		// Token: 0x040010A8 RID: 4264
		[DataField("bonkTime", false, 1, false, false, null)]
		public float BonkTime = 2f;

		// Token: 0x040010A9 RID: 4265
		[DataField("bonkDamage", false, 1, false, false, null)]
		public DamageSpecifier BonkDamage;
	}
}
