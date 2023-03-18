using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.TypingIndicator
{
	// Token: 0x02000608 RID: 1544
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("typingIndicator", 1)]
	public sealed class TypingIndicatorPrototype : IPrototype
	{
		// Token: 0x170003CE RID: 974
		// (get) Token: 0x060012EB RID: 4843 RVA: 0x0003E103 File Offset: 0x0003C303
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x040011A1 RID: 4513
		[DataField("spritePath", false, 1, false, false, null)]
		public ResourcePath SpritePath = new ResourcePath("/Textures/Effects/speech.rsi", "/");

		// Token: 0x040011A2 RID: 4514
		[DataField("typingState", false, 1, true, false, null)]
		public string TypingState;

		// Token: 0x040011A3 RID: 4515
		[DataField("offset", false, 1, false, false, null)]
		public Vector2 Offset = new Vector2(0.5f, 0.5f);

		// Token: 0x040011A4 RID: 4516
		[DataField("shader", false, 1, false, false, null)]
		public string Shader = "unshaded";

		// Token: 0x040011A5 RID: 4517
		[DataField("idleState", false, 1, true, false, null)]
		public string IdleState;
	}
}
