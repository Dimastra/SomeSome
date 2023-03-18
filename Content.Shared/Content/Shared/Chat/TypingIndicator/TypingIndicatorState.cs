using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator
{
	// Token: 0x0200060B RID: 1547
	[NetSerializable]
	[Serializable]
	public enum TypingIndicatorState
	{
		// Token: 0x040011AB RID: 4523
		None,
		// Token: 0x040011AC RID: 4524
		Idle,
		// Token: 0x040011AD RID: 4525
		Typing
	}
}
