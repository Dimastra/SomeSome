using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Server.Weapons.Ranged.Components
{
	// Token: 0x020000B5 RID: 181
	[RegisterComponent]
	public sealed class RangedDamageSoundComponent : Component
	{
		// Token: 0x040001F7 RID: 503
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("soundGroups", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageGroupPrototype>))]
		public Dictionary<string, SoundSpecifier> SoundGroups;

		// Token: 0x040001F8 RID: 504
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[DataField("soundTypes", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, DamageTypePrototype>))]
		public Dictionary<string, SoundSpecifier> SoundTypes;
	}
}
