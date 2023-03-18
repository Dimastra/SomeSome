using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Chat.Prototypes
{
	// Token: 0x0200060E RID: 1550
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("emoteSounds", 1)]
	public sealed class EmoteSoundsPrototype : IPrototype
	{
		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060012F0 RID: 4848 RVA: 0x0003E18C File Offset: 0x0003C38C
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x040011B9 RID: 4537
		[Nullable(2)]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier FallbackSound;

		// Token: 0x040011BA RID: 4538
		[DataField("params", false, 1, false, false, null)]
		public AudioParams? GeneralParams;

		// Token: 0x040011BB RID: 4539
		[DataField("sounds", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<SoundSpecifier, EmotePrototype>))]
		public Dictionary<string, SoundSpecifier> Sounds = new Dictionary<string, SoundSpecifier>();
	}
}
