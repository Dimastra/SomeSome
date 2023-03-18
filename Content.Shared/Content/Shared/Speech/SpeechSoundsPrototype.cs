using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Speech
{
	// Token: 0x0200017C RID: 380
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("speechSounds", 1)]
	public sealed class SpeechSoundsPrototype : IPrototype
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00011F12 File Offset: 0x00010112
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00011F1A File Offset: 0x0001011A
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00011F22 File Offset: 0x00010122
		[DataField("variation", false, 1, false, false, null)]
		public float Variation { get; set; } = 0.1f;

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00011F2B File Offset: 0x0001012B
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00011F33 File Offset: 0x00010133
		[DataField("saySound", false, 1, false, false, null)]
		public SoundSpecifier SaySound { get; set; } = new SoundPathSpecifier("/Audio/Voice/Talk/speak_2.ogg", null);

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x00011F3C File Offset: 0x0001013C
		// (set) Token: 0x06000494 RID: 1172 RVA: 0x00011F44 File Offset: 0x00010144
		[DataField("askSound", false, 1, false, false, null)]
		public SoundSpecifier AskSound { get; set; } = new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_ask.ogg", null);

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x00011F4D File Offset: 0x0001014D
		// (set) Token: 0x06000496 RID: 1174 RVA: 0x00011F55 File Offset: 0x00010155
		[DataField("exclaimSound", false, 1, false, false, null)]
		public SoundSpecifier ExclaimSound { get; set; } = new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_exclaim.ogg", null);
	}
}
