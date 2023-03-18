using System;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B0 RID: 1200
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundStartAttemptEvent : CancellableEntityEventArgs
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x0600186E RID: 6254 RVA: 0x0007F7DF File Offset: 0x0007D9DF
		public IPlayerSession[] Players { get; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x0600186F RID: 6255 RVA: 0x0007F7E7 File Offset: 0x0007D9E7
		public bool Forced { get; }

		// Token: 0x06001870 RID: 6256 RVA: 0x0007F7EF File Offset: 0x0007D9EF
		public RoundStartAttemptEvent(IPlayerSession[] players, bool forced)
		{
			this.Players = players;
			this.Forced = forced;
		}
	}
}
