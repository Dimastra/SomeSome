using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.GameObjects;

namespace Content.Server.Chat.Systems
{
	// Token: 0x020006C7 RID: 1735
	[NullableContext(1)]
	[Nullable(0)]
	[ByRefEvent]
	public struct EmoteEvent
	{
		// Token: 0x06002426 RID: 9254 RVA: 0x000BCA0E File Offset: 0x000BAC0E
		public EmoteEvent(EmotePrototype emote)
		{
			this.Emote = emote;
			this.Handled = false;
		}

		// Token: 0x04001665 RID: 5733
		public bool Handled;

		// Token: 0x04001666 RID: 5734
		public readonly EmotePrototype Emote;
	}
}
