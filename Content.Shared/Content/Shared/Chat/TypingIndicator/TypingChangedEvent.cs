using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator
{
	// Token: 0x02000605 RID: 1541
	[NetSerializable]
	[Serializable]
	public sealed class TypingChangedEvent : EntityEventArgs
	{
		// Token: 0x060012E8 RID: 4840 RVA: 0x0003E0D9 File Offset: 0x0003C2D9
		public TypingChangedEvent(TypingIndicatorState state)
		{
			this.State = state;
		}

		// Token: 0x0400119D RID: 4509
		public readonly TypingIndicatorState State;
	}
}
