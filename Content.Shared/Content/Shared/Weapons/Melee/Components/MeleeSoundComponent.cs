using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Weapons.Melee.Components
{
	// Token: 0x0200007B RID: 123
	[RegisterComponent]
	public sealed class MeleeSoundComponent : Component
	{
		// Token: 0x0400019C RID: 412
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("soundGroups", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageGroupPrototype>))]
		public Dictionary<string, SoundSpecifier> SoundGroups;

		// Token: 0x0400019D RID: 413
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("soundTypes", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageTypePrototype>))]
		public Dictionary<string, SoundSpecifier> SoundTypes;

		// Token: 0x0400019E RID: 414
		[Nullable(2)]
		[DataField("noDamageSound", false, 1, false, false, null)]
		public SoundSpecifier NoDamageSound;
	}
}
