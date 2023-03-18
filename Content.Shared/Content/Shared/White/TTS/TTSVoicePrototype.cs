using System;
using System.Runtime.CompilerServices;
using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.White.TTS
{
	// Token: 0x02000030 RID: 48
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("ttsVoice", 1)]
	public sealed class TTSVoicePrototype : IPrototype
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002B9D File Offset: 0x00000D9D
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002BA5 File Offset: 0x00000DA5
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002BAD File Offset: 0x00000DAD
		[DataField("sex", false, 1, true, false, null)]
		public Sex Sex { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002BB5 File Offset: 0x00000DB5
		[ViewVariables]
		[DataField("speaker", false, 1, true, false, null)]
		public string Speaker { get; } = string.Empty;

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002BBD File Offset: 0x00000DBD
		[DataField("roundStart", false, 1, false, false, null)]
		public bool RoundStart { get; } = 1;

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002BC5 File Offset: 0x00000DC5
		[DataField("sponsorOnly", false, 1, false, false, null)]
		public bool SponsorOnly { get; }
	}
}
