using System;
using System.Runtime.CompilerServices;
using Content.Shared.White.TTS;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.White.TTS
{
	// Token: 0x02000083 RID: 131
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TTSComponent : Component
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x0000ABEE File Offset: 0x00008DEE
		// (set) Token: 0x060001E8 RID: 488 RVA: 0x0000ABF6 File Offset: 0x00008DF6
		[ViewVariables]
		[DataField("voice", false, 1, false, false, typeof(PrototypeIdSerializer<TTSVoicePrototype>))]
		public string VoicePrototypeId { get; set; } = string.Empty;
	}
}
