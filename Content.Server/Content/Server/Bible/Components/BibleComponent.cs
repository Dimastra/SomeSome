using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Bible.Components
{
	// Token: 0x02000720 RID: 1824
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class BibleComponent : Component
	{
		// Token: 0x040017D3 RID: 6099
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x040017D4 RID: 6100
		[DataField("damageOnFail", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier DamageOnFail;

		// Token: 0x040017D5 RID: 6101
		[DataField("damageOnUntrainedUse", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier DamageOnUntrainedUse;

		// Token: 0x040017D6 RID: 6102
		[DataField("failChance", false, 1, false, false, null)]
		[ViewVariables]
		public float FailChance = 0.34f;

		// Token: 0x040017D7 RID: 6103
		[DataField("sizzleSound", false, 1, false, false, null)]
		public SoundSpecifier SizzleSoundPath = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", null);

		// Token: 0x040017D8 RID: 6104
		[DataField("healSound", false, 1, false, false, null)]
		public SoundSpecifier HealSoundPath = new SoundPathSpecifier("/Audio/Effects/holy.ogg", null);

		// Token: 0x040017D9 RID: 6105
		[DataField("locPrefix", false, 1, false, false, null)]
		public string LocPrefix = "bible";
	}
}
