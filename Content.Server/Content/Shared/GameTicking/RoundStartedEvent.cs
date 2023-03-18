using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking
{
	// Token: 0x02000013 RID: 19
	public sealed class RoundStartedEvent : EntityEventArgs
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002BFA File Offset: 0x00000DFA
		public int RoundId { get; }

		// Token: 0x06000038 RID: 56 RVA: 0x00002C02 File Offset: 0x00000E02
		public RoundStartedEvent(int roundId)
		{
			this.RoundId = roundId;
		}
	}
}
